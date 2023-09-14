# 持续构建工作流
SimulatorX 使用 TeamCity+Harbor+K8s 实现持续集成、持续部署、持续交付的工作流。
## TeamCity 监控仓库更新
## 服务端 & 客户端构建
采用 TeamCity Unity 工作流和多 TeamCity Agent运行本体构建
## 持续交付
### 客户端
构建完成后上传至内部对象存储，生成构建报告
### 服务端
构建完成本体后开始构建 Docker 镜像，上传至内部 Registry
## 构建报告
SimulatorX 项目自研 TeamCity-飞书 对接插件，实现了 TeamCity 构建报告、通知向飞书的发送
![arch-workflow-01.png](/static/images/arch-workflow-01.png)
![arch-workflow-02.png](/static/images/arch-workflow-02.png)
## 持续部署
SimulatorX 持续部署流程有两种部署模式
### TeamCity Deploy
利用 TeamCity 部署流程，传递 Artifact 在 Agent 部署
### K8s Registry
使用自建 Registry，在服务器k8s集群上自动拉取、更新