
# 同窗行迹


# 部署

## 数据库

<aside>

  本项目的数据库采用MySQL

</aside>

- SQL源文件为dump-classmate_trace-202411181526.sql，建议使用MySQL8.0及以上的版本

## 后端项目

<aside>

  本项目的后端以csharp项目为主体，辅以java项目，运行本项目时需启动csharp项目和java项目

</aside>

- 后端项目位于classmate_trace/BACK目录下
- csharp文件夹下存放csharp后端项目，建议使用Visual Studio运行与调试
- java文件夹下存放java后端项目，建议使用VSCode运行与调试

## 前端项目

<aside>

  本项目的前端为Vue项目

</aside>

- 前端项目位于classmate_trace/FRONT目录下
- 下载前端项目后，首先将web压缩包和node_modules压缩包解压，再将node_modules文件夹移动至web文件夹下，web文件夹即为完整的Vue前端项目，请按照web文件夹下的Markdown说明进行环境配置，建议使用VSCode运行与调试

## 前后端连接

<aside>

  本项目采取前后端分离开发的模式，前端项目、后端项目、数据库可以部署在同一本地，也可以分离部署，不同部署方式下的前后端连接与数据库连接方式有区别

</aside>

### 本地部署

- 在本地启动csharp文件夹下的back_local项目作为csharp后端
- 在本地启动java文件夹下的serve项目作为java后端
- 使用sql源文件在本地配置数据库
- 将back_local项目和serve项目中的数据库连接串的用户名、密码、数据库名称改为本地数据库的实际信息
- 在本地启动web项目作为Vue前端
- 将web项目中的接口请求url的IP地址改为localhost，http/https以及端口号需要参照back_local项目和serve项目的启动配置
- 若前端项目的接口响应解析存在错误，可能是返回数据的字段的首字母大小写在本地和分离部署时存在差异而导致的，修改出错部分的大小写即可

### 分离部署

- 在A机启动csharp文件夹下的back_server项目作为csharp后端，在Program.cs文件中为B机IP配置CORS网关
- 在B机启动java文件夹下的serve项目作为java后端
- 使用sql源文件在A机配置数据库，为B机创建一个角色user并给予足够权限
- 将back_server项目中的数据库连接串的用户名、密码、数据库名称改为A机数据库的实际信息
- 和serve项目中的数据库连接串的用户名、密码、数据库名称改为user的实际信息和A机的IP
- 在B机启动web项目作为Vue前端
- 将web项目中的csharp项目接口请求url的IP地址改为A机的IP，采用http；java项目接口请求url仍为localhost
- 若前端项目的接口响应解析存在错误，可能是返回数据的字段的首字母大小写在本地和分离部署时存在差异而导致的，修改出错部分的大小写即可

## 其他

### OSS云存储

<aside>

  前端项目在使用OSS云存储时需要阿里云的云服务器账号，建议使用自己的云服务器账号并配置好防火墙

</aside>
=======


