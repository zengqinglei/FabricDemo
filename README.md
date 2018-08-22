## 微服务示例项目
本示例项目主要结合当前比较流行的一些技术框架进行开发的，主要为微服务创建良好统一的构建环境。
通过本项目的示例，你将了解一个高可用、可拓展的基础项目结构等内容。

#### 使用框架如下：
* [Consul](https://www.cnblogs.com/zengqinglei/p/9367778.html): 集服务健康检查、服务注册、服务发现、kv存储(示例中使用apollo代替)。
* [Ocelot](https://github.com/ThreeMammals/Ocelot): API网关，利用consul 自动服务发现、负载，支持限流、降级、熔断、授权认证、，并可灵活的横向服务拓展。
* [Apollo](https://github.com/ctripcorp/apollo): 携程微服务配置中心，将项目中appsettings.json中配置全部转移至配置中心，可实现高可用、配置即时发布、灰度发布等。
* [IdentityServer4](https://github.com/IdentityServer/IdentityServer4): 授权认证框架，基于OAuth2、JWT认证协议标准，实现可用于不同场景的授权方式。
* [Docker](https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images): 项目使用docker容器进行部署，已编根据不同环境编写了不同环境的docker-compose.yml,可一键部署。
###### 基于以上框架，开发的组件包：
- [Creekdream.Configuration.Apollo](https://github.com/zengqinglei/Creekdream.Configuration.Apollo): 对接携程微服务配置中心。
- [Creekdream.Discovery.Consul](https://github.com/zengqinglei/Creekdream.Discovery.Consul): 与Consul对接，实现自动获取IP，注册服务以及健康检查。

#### 构建并启动项目
以下步骤接可参照我的博客园文章：[.NET CORE 微服务系列文章](https://www.cnblogs.com/zengqinglei/p/9348549.html)  
基于.net core 2.1 以及 centos7, ubuntu18.04, ubuntu16.04 均测试通过。
* **环境准备：**
  * 准备至少3台服务器或虚拟机
  * 安装Mssql数据库
  * 部署Consul服务
  * 部署Apollo配置中心
  * 安装docker以及docker-compose
* **构建以及部署：**
  * 将项目克隆至服务
  * 修改各项目中配置文件(appsetting.json)：数据库连接、配置中心
  * 打开项目设置FabricDemo.ApiGateway项目为启动项，使用ef自动迁移命令生成数据：  
    * ~~Add-Migration InitialConfiguration -Context ConfigurationDbContext~~  
    * ~~Add-Migration InitialPersistedGrant -Context PersistedGrantDbContext~~  
    * ~~Add-Migration InitialFabricDemo -Context FabricDemoDbContext~~  
    * // 以上均已执行，如需重新生成，请删除**Migrations**目录  
    * **Update-Database**
  * 微服务配置中心配置如下：
    * 新建Consul项目为公共配置项目，并添加如下简直配置：  
        ```
        {
            "ConsulClient:Address": "http://pro.consul.zengql.local"
        }
       ```
    * 新建FabricDemo.IdentityServer项目并管理公共项目配置：TEST1.ConsulPublic
        ```
        {
            "ConnectionStrings:Default": "server=192.168.0.105;database=FabricDemo;uid=sa;pwd=Sa123456",
            "ConsulService:ServiceName": "IdentityServer"
        }
       ```
    * 新建FabricDemo.ProductService项目并管理公共项目配置：TEST1.ConsulPublic
        ```
        {
            "ConsulService:ServiceName": "ProductService"
        }
       ```
    * 新建FabricDemo.UserService项目并管理公共项目配置：TEST1.ConsulPublic
        ```
        {
            "ConsulService:ServiceName": "UserService"
        }
       ```
    * 新建FabricDemo.ApiGateway项目并管理公共项目配置：TEST1.ConsulPublic
        ```
        {
            "IdentityServer:Authority": "http://pro.identityserver.zengql.local",
            "ReRoutes[0]:LoadBalancerOptions:Type": "RoundRobin",
            "ReRoutes[1]:LoadBalancerOptions:Type": "RoundRobin"
        }
       ```
  * 切换到项目根目并执行docker编排命令：docker-compose up --build
  * 其他服务无需再次编译部署步骤：
      * 将刚编译的镜像上传至阿里云或私有镜像服务器
      * 拷贝docker-compose*.yml 的3个文件至其他服务器
      * 在其他服务期修改docker-compose.pro.yml 文件，将image改为自己上传的镜像名称
      * 执行docker-compose -f docker-compose.override.yml -f docker-compose.pro.yml up -d

#
根据以上步骤完成后，整个微服务就部署完成了，效果如下图:  

**Consul服务中心**  
<img src="https://raw.githubusercontent.com/zengqinglei/FabricDemo/master/doc/images/consul.png" />
**微服务配置中心**  
<img src="https://raw.githubusercontent.com/zengqinglei/FabricDemo/master/doc/images/apollo.png" />
**Ocelot访问情况**  
<img src="https://raw.githubusercontent.com/zengqinglei/FabricDemo/master/doc/images/userservice.png" />
