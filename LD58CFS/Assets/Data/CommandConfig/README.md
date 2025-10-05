# 命令配置文件说明
+ commandName用于配置命令名称
+ params用于配置参数
  + param有三个属性
    + name
    + type，用于指定修饰器，email就是在后面加上@bmail.com
    + accept 用于指定可以接受的黑板键
  + case用于配置条件输出
    + 所有case必须以case开头，其他随便，建议case1-9往下走
    + name 
    + accept 用于指定可以匹配的值
    + 某个case中所有参数都匹配即为命中
    + 一次只能命中一个case
  + output
    + 实际输出的内容
    + ${param}可以引用变量，内置一些全局变量time，ip等，以及上面param块声明的变量（还没做）
    + $${func(param)}可以使用一些内置函数
      + $$wait(time) 暂停秒数
      + $$progress(totalTime,steps,originText,finalText,originColor,finalColor),可以对照WebScan看下用法
    + 使用case：修饰一整行（由于deepseek太菜懒得调只能先一整行用着了），只有前面条件命中的case才会显示
    + 使用chinese:或者english:修饰一个块,只有当游戏语言对应的时候才会输出
    + 使用clickable修饰一个块，用于指定点击后可以获取的字段