# Game Tutorial

## Level 1 — Phishing Emails & Web Scanning

### Background
Level 1 is a phishing-email + web-scanning scenario. The protagonist malware aims to trick the Bmail administrator into revealing credentials to log into the backend.

**Email phishing** is when an attacker impersonates a trusted sender (colleague, partner, bank, etc.) and uses email to lure recipients into clicking malicious links, opening attachments, or entering sensitive information such as usernames and passwords, thereby stealing data or delivering malware.

Common types include:
- **Link phishing**: embedding a forged URL in the message body to direct the user to a phishing site
- **Attachment phishing**: sending attachments that contain malicious macros or executables that run when opened
- **Clone phishing**: copying a legitimate email's content and injecting malicious elements, leveraging the recipient's trust in the original message
- **QR-code phishing**: placing a fake QR code in an email or message that directs users to a malicious site when scanned

**Real-world examples**: In the 2016 John Podesta phishing incident, attackers impersonated Google’s security team to steal the Democratic chairman’s credentials, influencing the U.S. election. Another high-profile case was the 2020 Twitter Bitcoin scam, where attackers used phishing to gain internal employee access and hijack celebrity accounts to post cryptocurrency scams.

**How to spot phishing emails**:
- Inspect the sender address: hover to reveal the actual email address
- Watch for urgent language: phrases like "click immediately" or "your account will be frozen"
- Check the URL: hover to reveal the real destination
- Avoid opening suspicious attachments unless the source is verified

In the game, Level 1 demonstrates link phishing. People tend to accept information that aligns with their interests or prior knowledge. Attackers exploit this psychology — for example, using a "cat pageant" link to prompt a quick click without careful inspection.

Clues about the protagonist in Level 1 can be found in browsing history and the recycle bin, which is a reminder that seemingly hidden places may contain sensitive information.

**Web scanning** uses crawlers or dedicated security tools to systematically fetch pages, enumerate paths, and detect vulnerabilities on a target site. It’s often used to discover hidden admin interfaces, unauthenticated APIs, and sensitive files — the game calls these "backend links."

The technique shown in the game is **directory/file enumeration**: trying common admin paths (e.g. `/admin/`, `/manager/`, `/login.php`) using wordlists or bruteforce. This is a classic approach and remains common in the real world. Mitigations include avoiding predictable paths, providing uniform error responses, and using a Web Application Firewall (WAF) to rate-limit requests.

**Defensive measures**:
- Use randomly generated admin path names
- Enforce IP whitelisting for backend access
- Deploy a Web Application Firewall (WAF)
- Perform regular security scans and monitoring

The main challenge in this level is assembling the Send-Email parameters: `OriginalLink` denotes the template link to use for the phishing page, and `Subject` should be chosen to pique the target’s interest.

### Solution
1. Gather email addresses from photos in the recycle bin
2. Retrieve cat pageant search data from history
3. Find `www.bmail.com` in link logs
4. Use WebScan to discover the backend link
5. Send a phishing email with Send-Email to obtain the password
6. Log into the backend using the discovered link

## Level 2 — SQL Injection & VPN Backdoor

### Background
Level 2 features an email system; directory/file enumeration still helps find HTTP interfaces to backend databases, and WebScan uncovers the corresponding database endpoints.

**SQL injection** occurs when a web application concatenates user-supplied input into SQL statements without proper sanitization or parameterization, allowing an attacker to inject SQL keywords (e.g. `SELECT`, `UNION`, `OR`, `--`) so the database executes queries it otherwise wouldn't.

Login forms, search boxes, URL parameters, and cookies are all potential injection points. For example, if the backend builds a query like `SELECT * FROM users WHERE name='${input}'`, an attacker can submit `' OR 1=1--` in `${input}` to make the condition always true, thereby bypassing authentication or retrieving all records.

SQL injection was first widely documented in 1998 (Phrack #54) and became a major web-security issue as PHP+MySQL and ASP+SQL Server proliferated in the 1999–2005 era. Even in 2025, SQL injection remains a standard item in penetration tests and audits; thousands of sites still suffer from improper protections. Parameterized queries, strict input validation, least privilege, and error concealment, together with regular pentests, can largely mitigate this risk.

**Severity levels for SQL injection**:
- **Low**: can read only public data
- **Medium**: can read sensitive data or modify records
- **High**: can execute system commands or gain full server control
- **Critical**: can delete databases or implant persistent backdoors

This level also uses phishing as in Level 1, but the goal shifts to injecting a backdoor — dropping script or binary payloads into the victim’s browser/system and achieving a network callback for remote control.

**Fingerprinting**: phishing pages may collect browser fingerprints (User-Agent, Canvas/WebGL traits, fonts, plugins, etc.) and store them, enabling the attacker to mimic a target's fingerprint to better impersonate the victim.

**C2 server (Command and Control)**: a remote server used by attackers to control compromised hosts. Backdoors typically poll the C2 for instructions and exfiltrate stolen data. C2 infrastructure can be:
- dedicated servers: attacker-rented VPS or cloud instances
- compromised hosts: otherwise legitimate servers that have been taken over
- P2P networks: decentralized control that is harder to shut down

Backdoors often tunnel through **VPNs** as a C2 channel because VPN encryption lets malicious traffic blend with legitimate traffic, making detection harder. The game’s **DPI (Deep Packet Inspection)** inspects not only headers (IP, ports) but also payload contents to identify protocols, patterns, and keywords.

**Real-world example**: The 2017 WannaCry ransomware spread using the EternalBlue exploit; its C2 infrastructure was hosted in North Korea. Attackers attempted to hide communications using various covert channels, contributing to a global outbreak.

**OpenVPN** is an open-source VPN implementation supporting UDP or TCP, TLS-based encryption (e.g. AES-256-CBC, AES-256-GCM), and multiple authentication modes (certificates, username/password, mutual TLS). It operates in TUN (routing) or TAP (bridging) modes.

**Backdoor persistence techniques**:
- **Startup registry entries**: adding keys in Windows registry to launch on boot
- **Scheduled tasks**: creating scheduled jobs that run at intervals
- **Service installation**: registering the backdoor as a system service
- **Kernel modules**: installing kernel-level backdoors that are hard to detect and remove

We note that Bmail stores passwords in plaintext, which is highly dangerous. In real systems, passwords should be processed with dedicated hashing algorithms combined with unique salts (so identical passwords do not yield identical hashes). Hashing compresses the password into a fixed-size "fingerprint" that is practically irreversible. Many additional protections are standard in modern systems; consult security references for details.

### Solution
1. Use WebScan to discover target endpoints
2. Use SQL injection to retrieve email records and key data
3. Inject the backdoor payload
4. Establish a VPN connection via Vpn-Tool
5. Use Remote-Connect to access the target system remotely

## Level 3 — Man-in-the-Middle & Firewall Bypass

### Background
Level 3 shifts from domain names to direct IPs and includes port targeting. In practice, you must first probe which ports on the target IP are open and which services (e.g. web servers) respond to HTTP requests.

This level introduces **MITM (Man-in-the-Middle)** attacks, where an attacker inserts themselves between two communicating parties (typically client ↔ server), intercepting, modifying, or forging network traffic while both sides believe they are communicating directly.

**MITM implementation techniques**:
- **ARP spoofing**: forging ARP responses on a LAN to redirect traffic
- **DNS spoofing**: altering DNS resolution to point victims to fake sites
- **BGP hijacking**: intercepting routes at the internet routing level
- **SSL stripping**: downgrading HTTPS to HTTP
- **Malicious proxies**: tricking clients to use an attacker-controlled proxy

**How ARP spoofing works**:
ARP (Address Resolution Protocol) maps IP addresses to MAC addresses. When host A wants to talk to host B, it broadcasts an ARP request for B’s MAC. An attacker C can send a forged ARP reply claiming to be B’s MAC, causing A to send packets destined for B to the attacker instead.

**Real-world examples**: In 2018 an attack on Marriott hotel Wi‑Fi used ARP spoofing to intercept guests' traffic and steal credit card and login data. Another example is the 2019 Capital One breach, where cloud misconfiguration and MITM techniques contributed to the theft of data for millions of users.

**MITM defensive measures**:
- Use HTTPS and HSTS (HTTP Strict Transport Security)
- Deploy ARP monitoring and detection
- Use VPNs to encrypt traffic
- Implement certificate pinning
- Keep systems patched and up to date

The attack code in the level exploits a **buffer overflow in an authentication module**, meaning a component responsible for login/credential checks has a memory buffer vulnerability. Overwriting the buffer leads to control-flow corruption.

**Buffer overflow mechanics**:
Programs allocate fixed-size memory for variables. When input exceeds that size, excess data overwrites adjacent memory. A carefully crafted input can overwrite return addresses, alter variables, or ultimately execute arbitrary code.

With ARP spoofing redirecting traffic to the attacker, packet-capture tools (Wireshark, tcpdump) can reveal plaintext or weakly encrypted authentication data, allowing credential theft — which is what this level obtains.

**Hash harvesting**: searching the target system for known hash values (MD5, SHA-1, SHA-256) to locate corresponding files or processes such as authentication services (`radiusd`, `tac_plus`) or their configuration files for later tampering.

**Brute force**: attempting many password guesses until success. Methods include pure brute force (exhaustive character combinations) and dictionary+rule attacks, which apply transformations to likely password lists.

**RADIUS/TACACS+ protocols**:
- **RADIUS** (Remote Authentication Dial-In User Service): used for network access control (authentication, authorization, accounting)
- **TACACS+**: a Cisco protocol offering finer-grained access control

In large enterprises, carriers, and cloud providers these protocols are less often misused; but smaller organizations, campuses, labs, and legacy devices may still rely on RADIUS/TACACS+ without strong password policies, encrypted transports, or multi-factor authentication.

The story premise here is that a security company implanted a backdoor into Bwiiter but our little malware turns that backdoor against them.

### Solution
1. Use WebScan to enumerate the target
2. Use SQL injection against `vuln-report`
3. Obtain the target IP
4. Perform MITM to capture credentials (e.g. `gugu-admin`)
5. Bypass the firewall to retrieve the password
6. Log in successfully

## Level 4 — Backdoors in Security Companies

### Background
Level 4 does not introduce new attack mechanics but surfaces interesting real-world information.

It is not uncommon for security vendors or other service providers to ship software that contains backdoors or exfiltration capabilities:

- **Hacking Team (2018)**: an Italian spyware vendor was exposed for embedding backdoors in its products and selling surveillance tools to repressive regimes. Leaked emails showed assistance to the Sudanese government, which led to arrests.

- **NetSarang (2021)**: researchers discovered a backdoor module (`nssock2.dll`) in NetSarang’s Xshell/Xmanager SSH clients. The module could collect and upload user and system data, affecting millions of users.

- **SolarWinds Orion supply-chain attack (2020)**: a Russian APT compromised SolarWinds’ build infrastructure and inserted the Sunburst backdoor into legitimate updates for the Orion monitoring product. Over 18,000 customers, including sensitive U.S. agencies, received the compromised update, enabling extensive espionage.

**Supply-chain attack consequences**:
- **Trust erosion**: customers lose faith in software vendors
- **Economic cost**: organizations must invest heavily in audits and remediation
- **Data leakage**: sensitive information can be stolen and later weaponized
- **Regulatory scrutiny**: legal and oversight consequences for affected parties

**Other notable incidents**:
- **Okta library backdoor (2022)**: a support library was found to contain backdoor-like code
- **CCleaner (2017)**: a popular system-cleaning tool was distributed with a backdoor affecting millions
- **Supremo RAT (2015)**: remote-access software discovered with an embedded backdoor used for surveillance

**Ethical concerns around security-vendor backdoors**:
- **Dual use**: surveillance tools can serve legitimate law-enforcement purposes but also be abused
- **Lack of transparency**: users are often unaware of monitoring capabilities
- **National security vs. privacy**: tension between government needs and civil liberties
- **Responsibility**: who is accountable when a vendor-supplied backdoor is abused?

**Mitigations**:
- Prefer open-source software where code can be audited
- Rely on digital signature verification to ensure software integrity
- Isolate critical systems (air-gapping) when appropriate
- Commission third-party audits and assessments
- Apply least-privilege principles to limit software access

### Solution
1. Use WebScan to enumerate the target
2. Use SQL injection to extract `client-data`
3. Perform MITM to discover IP addresses
4. Log in successfully

## Level 5 — IoT Exploitation & Zero-Day Vulnerabilities

### Background
One of the new tools in this level is **IOT-Exploit**, which targets software/hardware defects, misconfigurations, or protocol weaknesses in Internet-of-Things (IoT) devices. Attackers develop or reuse code and scripts to gain unauthorized control, implant backdoors, and pivot laterally (using compromised devices as footholds to access internal networks or cloud services).

Today, billions of devices — cameras, routers, industrial controllers, smart-home devices, and vehicle systems — are deployed on public IPs or reachable behind NAT, often with poor security design (weak default passwords, lack of encryption, etc.).

**IoT security issues**:
- **Default credentials**: many devices ship with `admin/admin`-style defaults
- **Firmware vulnerabilities**: vendors seldom provide timely updates
- **Weak encryption**: outdated ciphers or plaintext communication
- **Exposed services**: unnecessary internet-facing interfaces
- **Supply-chain risks**: third-party components may contain backdoors

**Notable IoT attack cases**:
- **Mirai botnet (2016)**: infected hundreds of thousands of IoT devices (cameras, routers) to launch one of the largest DDoS attacks, impacting services like Twitter and Netflix. Infection relied on brute-forcing weak credentials.
- **VPNFilter (2018)**: malware that backdoored routers to steal data, launch DDoS attacks, and self-destruct; it affected around 500,000 devices worldwide.
- **2020 IoT botnet attacks**: large-scale DDoS using smart-home devices reached peak traffic of 1.5 Tbps.

In the game, an intelligent thermostat is used as a pivot to discover internal domain names and the internal DNS server used for name resolution. If DNS services are modified or replaced, traffic can be redirected, internal services impersonated, or sensitive information leaked.

**DNS spoofing** in the game refers to altering domain name resolution so that users are directed to an incorrect IP address — i.e., they believe they are visiting a legitimate site but are routed to an attacker-controlled server. Two common techniques are:

- **Cache poisoning**: forging DNS responses so that recursive resolvers cache incorrect records and serve them to subsequent queries
- **Man-in-the-middle tampering**: intercepting genuine DNS queries and returning forged responses

**DNSSEC** can mitigate such attacks, but the in-game base has not enabled it. **TTL manipulation** is the practice of modifying or abusing a record’s TTL (time-to-live) — expressed in seconds — to control how long caches retain a poisoned record.

**Gateway**: colloquially a network "door" or relay. In VPN contexts, a VPN gateway authenticates remote users/devices, establishes encrypted tunnels, forwards tunnel traffic, and pushes routing/DNS/policy information to clients.

The game’s **vpn-forge** simulates forging or emulating a VPN gateway or session — essentially creating an entity that appears to be a legitimate VPN endpoint, convincing the client or upstream devices that it is a trusted peer and thereby gaining access to traffic or permissions.

Even **physically air-gapped systems** can be attacked. An attacker that controls a device connected to a management or office network (printer, copier, scanner, desktop, laptop, etc.) can collect network topology, share pathways, reachable hosts, and available peripherals (USB, file shares). They look for ways to write malware to removable media or peripherals (printer storage, USB drives). Those media can then be carried into the air-gapped environment and introduced to isolated systems. If the isolated systems autorun media or lack strong integrity checks, infection becomes possible.

**Do air-gapped devices have IP addresses?** Many so-called "air-gapped" environments are actually "controlled gaps" with dedicated management networks. Devices on those management networks often have IPs for remote maintenance or logging; some isolation workflows temporarily connect isolated machines to IP-enabled devices (e.g., via USB-to-Ethernet bridges). During those maintenance windows the isolated environment is exposed at the network layer.

**Zero-day discovery and exploitation process**:
1. **Discovery**: find software defects via reverse-engineering, fuzzing, or code review
2. **Proof-of-concept (PoC)**: create a PoC to confirm the vulnerability is exploitable
3. **Exploit development**: craft reliable exploit code
4. **Weaponization**: integrate the exploit into malware or attack tooling
5. **Deployment**: execute the attack against target systems

**Famous zero-day examples**:
- **Heartbleed (2014)**: an OpenSSL flaw that allowed reading server memory and leaking sensitive data
- **WannaCry (2017)**: used the EternalBlue SMB zero-day to spread ransomware globally
- **Log4Shell (2021)**: a remote code execution vulnerability in Apache Log4j that affected millions of systems

At the end of the game, a tool called **ZeroDay-Exploit** is used. "Zero-Day" denotes a vulnerability unknown to the vendor or public and without a patch; a Zero-Day Exploit is code that takes advantage of such a flaw.

**Binary analysis** is the static or dynamic investigation of compiled machine code — examining disassembly or decompiled output, control-flow graphs, call graphs, symbols, and strings to understand function interactions. Malware and defensive protections often use anti-debugging techniques to detect debuggers, VMs, or analysis environments and change behavior to evade inspection.

**Buffer overflow**: writing more data to a buffer than it can hold overwrites adjacent memory; if an attacker controls that overwritten data, they can alter program control flow.

**Kernel injection / kernel-mode privilege escalation**: the OS kernel has the highest privileges. If an attacker can execute code in kernel mode or modify kernel data structures, they can bypass most user-mode defenses, install rootkits, hide processes, or escalate privileges. Kernel attacks are more severe and harder to detect and remediate than user-mode attacks.

**Attack vector**: the path or means by which an attacker reaches or compromises a target (network services, malicious attachments, removable media, driver bugs, etc.).

### Solution
1. Use SQL injection
2. Use IoT-Exploit to leverage IoT device vulnerabilities
3. Use DNS-Spoof to manipulate name resolution
4. Use VPN-Forge to fake a VPN gateway
5. Use USB-Infect to spread via removable media
6. Use Zero-Day to exploit an unpatched vulnerability
7. Use Nuke-Control to access the nuclear control system
