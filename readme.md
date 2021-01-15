# [Obsolete] Tup.Cobar4Net

- Cobar 是提供关系型数据库 (MySQL) 分布式服务的中间件, 它可以让传统的数据库得到良好的线性扩展, 并看上去还是一个数据库, 对应用保持透明.
- 本项目为 Alibaba [Cobar](https://github.com/alibaba/cobar/) .NET 实现版.

## 目前功能:
1.  [MySQL Sql Parser][2]
2.  [Config][3]
3.  [Route][4]

## Roadmap
* 0.1 完成 SqlParser+Route.
* 0.2 去掉遗留 JAVA 语法特征, 整理代码符合 C#/.NET 规范.
* 0.3 实现一组简单 ADO.NET DbProviderFactory 封装类,  可以直连方式使用 Route 功能. 作为一个基本 MySQL  Sharding 方案来使用.
* 0.5 待定...
* 1.0 实现 Cobar Server 完整功能.
* 1.1 实现 Cobar Manager 功能.

## 相关资料:
*  Cobar wiki [https://github.com/alibaba/cobar/wiki](https://github.com/alibaba/cobar/wiki)
*  Cobar 文档 [https://github.com/alibaba/cobar/tree/master/doc](https://github.com/alibaba/cobar/tree/master/doc)

## 环境:
*  .NET 4.0/VS2015

## 第三方:
*  Sharpen [Automated Java->C# coversion](https://github.com/mono/sharpen)
*  Deveel Math [A library for handling big numbers and decimals under Mono/.NET frameworks](https://github.com/deveel/deveel-math)

## license:
*  [Apache 2.0 license][1]

## Developed By:
* TUPUNCO - <tupunco@gmail.com>

[1]: LICENSE.txt
[2]: https://github.com/tupunco/Tup.Cobar4Net/tree/master/Tup.Cobar4Net/Parser
[3]: https://github.com/tupunco/Tup.Cobar4Net/tree/master/Tup.Cobar4Net/Config
[4]: https://github.com/tupunco/Tup.Cobar4Net/tree/master/Tup.Cobar4Net/Route

