-- MySQL dump 10.13  Distrib 8.0.35, for Win64 (x86_64)
--
-- Host: localhost    Database: classmate_trace
-- ------------------------------------------------------
-- Server version	8.0.35

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `comments`
--

DROP TABLE IF EXISTS `comments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comments` (
  `id` int NOT NULL AUTO_INCREMENT,
  `wallId` int NOT NULL COMMENT '对应留言的ID',
  `userId` varchar(100) NOT NULL COMMENT '评论者ID',
  `imgurl` varchar(100) DEFAULT NULL COMMENT '头像路径',
  `comment` varchar(1000) DEFAULT NULL COMMENT '评论内容',
  `name` varchar(100) NOT NULL COMMENT '用户名',
  `moment` varchar(100) NOT NULL COMMENT '时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comments`
--

LOCK TABLES `comments` WRITE;
/*!40000 ALTER TABLE `comments` DISABLE KEYS */;
INSERT INTO `comments` VALUES (1,1,'9','0','毕业快乐！！！(๑>؂<๑）','楚','2024-07-15'),(2,2,'6','2','愿我们前程似锦！！','Fabulous','2024-10-25'),(3,4,'9','3','未来可期！✧٩(ˊωˋ*)و✧','欢西西','2024-8-18'),(4,2,'7','4','苟富贵，勿相忘~(σ′▽‵)′▽‵)σ','azan','2024-10-24'),(5,3,'11','2','雄关漫道真如铁，而今迈步从头越。','回笼教主','2024-06-30'),(6,5,'1','6','老师您辛苦了！','a.m.','2024-09-01'),(7,7,'10','7','愿你如星星般有棱有角，继续发光！','糊米酒','2024-10-02'),(8,1,'11','8','祝愿我们殊途同归，再见时谈笑风生，分享来时的风景','新月坨吗头','2024-07-16'),(9,6,'8','6','愿我们前程似锦！！','孜然','2024-09-10'),(10,3,'13','5','书上说，月有阴晴圆缺，天下无不散的宴席，结束的是这盛夏，而不是我们，希望大家都能共赴星辰大海，此去不今年，后会终有期。','想尔','2024-07-01'),(11,6,'3','4','毕业快乐！！！','当当','2024-09-12'),(12,5,'7','2','人间骄阳正好，风过林梢，彼时我们正当年少','。','2024-09-05'),(13,7,'12','9','未来可期！(*˘︶˘*).｡.:*♡','potato','2024-10-05'),(14,8,'8','0','顶你！','匿名','2024-11-17T15:05:55.818Z'),(15,6,'16','7','祝我们都越来越好！','匿名','2024-11-18T03:30:18.232Z'),(16,18,'16','11','顶1','匿名','2024-11-18T03:33:27.296Z'),(17,17,'16','2','好耶！','匿名','2024-11-18T04:23:52.162Z'),(18,18,'16','0','太好看了','匿名','2024-11-18T04:28:20.162Z'),(19,17,'16','9','很好！','匿名','2024-11-18T07:14:12.252Z');
/*!40000 ALTER TABLE `comments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `feedbacks`
--

DROP TABLE IF EXISTS `feedbacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feedbacks` (
  `id` int NOT NULL AUTO_INCREMENT,
  `wallId` int NOT NULL COMMENT '对应留言的ID',
  `userId` varchar(100) NOT NULL COMMENT '反馈者ID-游客直接记录ip地址',
  `type` int NOT NULL COMMENT '反馈类型0喜欢1举报2撤销',
  `moment` varchar(100) NOT NULL COMMENT '时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `feedbacks`
--

LOCK TABLES `feedbacks` WRITE;
/*!40000 ALTER TABLE `feedbacks` DISABLE KEYS */;
INSERT INTO `feedbacks` VALUES (1,1,'5',0,'2024-07-17'),(2,4,'9',0,'2024-08-25'),(3,5,'1',0,'2024-08-22'),(4,3,'13',0,'2024-07-11'),(5,6,'3',0,'2024-09-25'),(6,7,'1',0,'2024-09-30'),(7,2,'7',0,'2024-10-15'),(8,7,'2',0,'2024-10-11'),(9,3,'12',0,'2024-07-01'),(10,2,'8',0,'2024-11-05'),(11,4,'14',0,'2024-09-12'),(12,1,'9',0,'2024-07-23'),(13,6,'7',0,'2024-09-14'),(14,6,'8',0,'2024-11-17T15:00:03.893Z'),(15,8,'16',0,'2024-11-18T03:29:49.407Z'),(16,18,'16',0,'2024-11-18T03:33:14.823Z'),(17,6,'16',0,'2024-11-18T04:23:44.428Z'),(18,20,'16',0,'2024-11-18T04:32:30.529Z'),(19,13,'16',0,'2024-11-18T05:34:36.642Z'),(20,16,'16',0,'2024-11-18T07:13:35.701Z'),(21,17,'16',0,'2024-11-18T07:14:15.601Z');
/*!40000 ALTER TABLE `feedbacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `walls`
--

DROP TABLE IF EXISTS `walls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `walls` (
  `id` int NOT NULL AUTO_INCREMENT,
  `classId` int NOT NULL,
  `type` int NOT NULL COMMENT '0留言1相册',
  `message` varchar(1000) DEFAULT NULL,
  `name` varchar(100) NOT NULL COMMENT '昵称',
  `userId` varchar(100) NOT NULL,
  `moment` varchar(100) NOT NULL,
  `label` int NOT NULL,
  `color` int DEFAULT NULL,
  `imgurl` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `walls`
--

LOCK TABLES `walls` WRITE;
/*!40000 ALTER TABLE `walls` DISABLE KEYS */;
INSERT INTO `walls` VALUES (1,3,0,'毕业季，我们收获了知识，也收获了友谊。愿我们在未来的道路上，不忘初心，砥砺前行，共创美好未来！','半晴云暮','13','2024-07-15',3,0,''),(2,1,0,'亲爱的同学们，转眼间我们就要毕业了，感谢这四年的陪伴和友谊，祝愿大家前程似锦！','逸归圣师','15','2024-10-23',2,1,''),(3,3,0,'毕业不是结束，而是新的开始。希望我们都能在未来的道路上，勇往直前，实现自己的梦想！','okei','14','2024-06-30',3,2,''),(4,3,0,'班级里的每一个瞬间都值得珍藏，感谢大家在这四年里给予的支持和帮助，期待未来再相聚！','Art','5','2024-08-17',4,1,''),(5,2,0,'毕业季，告别校园，但我们的友谊长存。愿我们在未来的日子里，都能找到自己的方向，勇攀高峰！','心肺复苏','9','2024-08-21',1,2,''),(6,1,0,'感谢老师们的辛勤教导，让我们在知识的海洋里畅游。祝愿老师们身体健康，工作顺利！','时芜','6','2024-09-04',2,3,''),(7,2,0,'时光荏苒，四年时光转瞬即逝。感谢同学们的陪伴，让我们在成长的道路上不再孤单。愿大家前程似锦，未来可期！','一些zkkz','5','2024-09-30',1,3,''),(8,1,0,'第一次使用这个系统，感觉好好用！','史非','8','2024-11-17T15:05:35.352Z',0,4,''),(9,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T01:36:21.598Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy32f735c7-d8b8-4d1c-b3a4-14f9232021e2'),(11,1,1,'大家一起在南岸嘴江滩公园散步','2024.11.16的小聚','16','2024-11-18T01:55:42.590Z',8,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy3cb1cb2a-c1c3-4f5a-8dc4-abbb157e15c4'),(12,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T01:57:05.269Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyee42eedd-b261-4e01-b5e3-388191d61fa0'),(13,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T01:59:48.434Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye4c6e922-7c84-42be-933b-b8ba5a603db0'),(14,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T02:21:33.692Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcya99a4210-4252-4a14-affe-665db3ab15f7'),(15,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T02:29:42.295Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy1c678fd1-ef02-413e-98af-78db33e7fcab'),(16,1,1,'20年后再相会','2024.11.19的小聚','16','2024-11-18T03:26:57.228Z',14,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyd45dba65-7bfe-4209-a635-33ea489e88ee'),(17,1,0,'终于结束所有专必的学习啦！','前端战士','16','2024-11-18T03:31:44.640Z',8,1,''),(18,1,1,'美丽的风景','史非','16','2024-11-18T03:32:48.022Z',3,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye4b40d0f-194d-41d4-bed4-1e1b77d6aed3'),(19,1,0,'最后在校的一年啦','史非','16','2024-11-18T04:24:20.028Z',3,2,''),(20,1,1,'一起来看路演吧！','2024.11.24的小聚','16','2024-11-18T04:32:22.866Z',17,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcybaad2e9e-9ece-478f-b224-2fae89005d05'),(21,1,1,'一起来看晚会','2024.11.24的小聚','16','2024-11-18T05:33:08.556Z',16,5,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye00c4d0f-8c4a-4974-bc2d-2d81dba14274');
/*!40000 ALTER TABLE `walls` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `个人留言`
--

DROP TABLE IF EXISTS `个人留言`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `个人留言` (
  `个人留言ID` int NOT NULL AUTO_INCREMENT COMMENT '系统分配，隐',
  `被留言同学ID` int NOT NULL,
  `留言同学ID` int NOT NULL,
  `留言信息` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`个人留言ID`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `个人留言`
--

LOCK TABLES `个人留言` WRITE;
/*!40000 ALTER TABLE `个人留言` DISABLE KEYS */;
INSERT INTO `个人留言` VALUES (1,1,8,'嗨，好久不见，最近怎么样？'),(2,15,7,'记得我们那时候一起打篮球的日子，好怀念啊！'),(3,1,9,'今天看到你发的照片，还是那么漂亮，变化不大呢！'),(4,10,5,'听说你换了新工作，恭喜啊！希望一切顺利！'),(5,5,9,'记得周末聚会哦，我们很久没聚了！'),(6,7,15,'上次聚会太有趣了，下次还要一起玩得更疯狂！'),(7,14,13,'看到你分享的学习心得，受益匪浅，感谢分享！'),(8,8,6,'最近在忙些什么呢？有空出来喝杯咖啡聊聊天吗？'),(9,8,3,'你的旅行照片真美，期待你回来后分享更多故事！'),(10,5,10,'今天心情不好，谢谢你发来的鼓励，感觉好多了！'),(11,16,16,'第一次使用系统，冒个泡！');
/*!40000 ALTER TABLE `个人留言` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `位置`
--

DROP TABLE IF EXISTS `位置`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `位置` (
  `同学ID` int NOT NULL,
  `最新位置信息` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '按地图定位需求存储，数据类型暂定',
  `历史位置轨迹线` varchar(10000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '位置信息列表，空格分割',
  `经度` int DEFAULT NULL,
  `纬度` int DEFAULT NULL,
  `历史经纬度轨迹线` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`同学ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `位置`
--

LOCK TABLES `位置` WRITE;
/*!40000 ALTER TABLE `位置` DISABLE KEYS */;
INSERT INTO `位置` VALUES (1,'吉林省四平市伊通满族自治县黄岭子镇504国道','湖北省武汉市黄陂区盘龙城经济开发区未来海岸D区-V2-2栋 吉林省四平市伊通满族自治县黄岭子镇504国道',NULL,NULL,NULL),(2,'四川省成都市武侯区桂溪街道锦晖西一街布鲁明顿广场','湖北省武汉市江岸区后湖街道金桥大道宏宇绿色新都 四川省成都市锦江区柳江街道交子VC露营基地 北京市房山区南窖乡桃山路 四川省成都市武侯区桂溪街道锦晖西一街布鲁明顿广场',NULL,NULL,NULL),(3,'陕西省咸阳市渭城区正阳街道汉韵七路','湖北省武汉市江岸区四唯街道汉口江滩绿道汉口江滩2期 陕西省咸阳市渭城区正阳街道汉韵七路',NULL,NULL,NULL),(4,'贵州省贵阳市观山湖区朱昌镇牟老贵阳百花湖风景名胜区','湖北省武汉市硚口区古田街道融侨锦城 湖南省长沙市雨花区圭塘街道万家丽中路辅路美洲故事西区 贵州省贵阳市观山湖区朱昌镇牟老贵阳百花湖风景名胜区',NULL,NULL,NULL),(5,'浙江省杭州市上城区彭埠街道新风路83号杭州东站','湖北省武汉市硚口区韩家墩街道三星公寓(香港映象东北)古田五路7号大院 云南省昆明市西山区福海街道陆家路北京芳草地国际学校(昆明分校) 西藏自治区拉萨市城关区纳金街道西藏惠诚嘉里商贸有限公司 浙江省杭州市上城区彭埠街道新风路83号杭州东站',NULL,NULL,NULL),(6,'内蒙古自治区呼和浩特市武川县哈乐镇','湖北省武汉市武昌区杨园街道长江 内蒙古自治区呼和浩特市武川县哈乐镇',NULL,NULL,NULL),(7,'上海市浦东新区三林镇联明路102号','湖北省武汉市汉阳区龙阳街道杨泗港快速路欧亚达汉阳国际广场 上海市浦东新区三林镇联明路102号',NULL,NULL,NULL),(8,'江苏省南京市鼓楼区热河南路街道热河南路白云小区','湖北省武汉市武昌区徐家棚街道学府路湖北大学(武昌校区) 河南省郑州市中原区航海西路街道五世名厨豫菜馆(秦岭路店)聚龙花园 江苏省南京市玄武区孝陵卫街道永丰大道华能国际能源先行区(总部) 新疆维吾尔自治区乌鲁木齐市乌鲁木齐县托里乡G30连霍高速 黑龙江省绥化市肇东市里木店镇滨洲线 上海市浦东新区塘桥街道浦建路715号威立雅有限公司 香港特别行政区离岛区新10号码头香港国际机场 江苏省南京市鼓楼区热河南路街道热河南路白云小区',NULL,NULL,NULL),(9,'福建省三明市建宁县客坊乡荷树下','湖北省武汉市汉阳区晴川街道龟山北路1-2号楼汉阳造广告创意园 广西壮族自治区柳州市柳江区百朋镇 福建省三明市建宁县客坊乡荷树下',NULL,NULL,NULL),(10,'浙江省丽水市松阳县大东坝镇','湖北省武汉市硚口区长丰街道长丰大道863号长丰城H4区 西藏自治区昌都市芒康县徐中乡行巴 浙江省丽水市松阳县大东坝镇',NULL,NULL,NULL),(11,'海南省海口市秀英区长流镇长彤路五源河国家湿地公园','湖北省武汉市武昌区水果湖街道省发改委公寓中国科学院武汉物理与数学研究所 海南省海口市秀英区长流镇长彤路五源河国家湿地公园',NULL,NULL,NULL),(12,'内蒙古自治区阿拉善盟阿拉善左旗巴润别立镇峡子沟贺兰山自然保护区','湖北省武汉市东西湖区柏泉街道东西湖堤 甘肃省定西市临洮县上营乡111县道 内蒙古自治区阿拉善盟阿拉善左旗巴润别立镇峡子沟贺兰山自然保护区',NULL,NULL,NULL),(13,'四川省成都市龙泉驿区龙泉街道柏林千和居小区柏林小区','湖北省武汉市黄陂区天河街道陈叶塆 四川省成都市龙泉驿区龙泉街道柏林千和居小区柏林小区',NULL,NULL,NULL),(14,'河南省新乡市获嘉县太山镇大张卜商店','湖北省武汉市洪山区青菱街道菱湖钓鱼场 福建省福州市晋安区寿山乡水尾厝 香港特别行政区屯门区龙珠岛东座别墅龙珠花园 湖北省武汉市黄陂区武湖街道滠阳大道 河南省新乡市获嘉县太山镇大张卜商店',NULL,NULL,NULL),(15,'河南省郑州市中原区沟赵乡枫香街天健湖公园','湖北省武汉市江夏区江夏区经济开发区庙山街道汤孙湖旅游度假区 河南省郑州市中原区沟赵乡枫香街天健湖公园',NULL,NULL,NULL),(16,'湖北省武汉市东西湖区将军路街道金银潭大道36号黄塘湖公园','湖北省武汉市东西湖区将军路街道金银潭大道36号黄塘湖公园',NULL,NULL,NULL);
/*!40000 ALTER TABLE `位置` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `同学`
--

DROP TABLE IF EXISTS `同学`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `同学` (
  `同学ID` int NOT NULL,
  `电话` varchar(100) NOT NULL COMMENT '账户名',
  `密码` varchar(100) NOT NULL,
  `姓名` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '个人信息',
  `生日` date DEFAULT NULL COMMENT '个人信息',
  `邮箱` varchar(100) DEFAULT NULL COMMENT '个人信息',
  `签名` varchar(100) DEFAULT NULL COMMENT '个人信息',
  `班级ID列表` varchar(100) DEFAULT NULL COMMENT '空格分隔',
  `最后登录时间` date DEFAULT NULL,
  `性别` int DEFAULT NULL COMMENT '男；女；未知',
  `头像` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`同学ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `同学`
--

LOCK TABLES `同学` WRITE;
/*!40000 ALTER TABLE `同学` DISABLE KEYS */;
INSERT INTO `同学` VALUES (1,'14798805283','P@ssw0rd!','张泽浩',NULL,'389504812@qq.com','爱生活，爱微笑。','1 2','2024-11-18',3,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy32f735c7-d8b8-4d1c-b3a4-14f9232021e2'),(2,'18170422280','Qwerty$123','沈炯延','2002-12-31',NULL,'追寻简单，享受当下。','2','2024-11-18',2,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcycdab6164-42ab-45c2-97e3-280195639098'),(3,'13053631921','1234567890!@','肖琪琪',NULL,'586937421@qq.com',NULL,NULL,'2024-11-15',3,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy3cb1cb2a-c1c3-4f5a-8dc4-abbb157e15c4'),(4,'19192229844','AbcdEFgHiJkLmnoP','王靖晶',NULL,NULL,'读书破万卷，下笔如有神。','2','2024-11-18',2,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyee42eedd-b261-4e01-b5e3-388191d61fa0'),(5,'16315352703','QazWSxEdCrFvTgbNhy','张淼','2003-01-23','210395876@qq.com','遇见更好的自己，每一天都是新的开始。','2 3','2024-11-18',0,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye4b40d0f-194d-41d4-bed4-1e1b77d6aed3'),(6,'14219781882','1q2w3e4r5t6y7u8i9o0p','黄洁妮',NULL,NULL,NULL,NULL,'2024-11-13',1,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyd45dba65-7bfe-4209-a635-33ea489e88ee'),(7,'15975899656','Password123!','栾梦妍','2003-03-10','897856123@qq.com',NULL,'1 2','2024-11-18',0,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy1c678fd1-ef02-413e-98af-78db33e7fcab'),(8,'17844884379','Welcome1@','王梓楠','2003-02-20','1290757830@qq.com',NULL,'1','2024-11-18',1,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyb50ca9ee-c085-4598-afcb-2ae1e03ebfbf'),(9,'13988507810','Qwertyuiop!','周煜博',NULL,'653748921@qq.com','心怀感激，生活更美好。','3','2024-11-16',3,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcya99a4210-4252-4a14-affe-665db3ab15f7'),(10,'19743913189','Asdfghjkl!','郭敏义',NULL,'912365476@qq.com',NULL,'2','2024-11-16',1,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcybaad2e9e-9ece-478f-b224-2fae89005d05'),(11,'14692499454','Zxcvbnm1@','秦雅轩','2003-04-01',NULL,'热爱生活，热爱工作，热爱一切美好事物。','3','2024-11-18',3,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcya99a4210-4252-4a14-affe-665db3ab15f7'),(12,'18081705118','1234567890!','李乐泉','2002-10-24',NULL,NULL,'2 3','2024-11-17',2,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy3cb1cb2a-c1c3-4f5a-8dc4-abbb157e15c4'),(13,'19904310782','AbcdEFgHiJkLmnoP!','陈婉昱',NULL,'712037495@qq.com','保持好奇，探索未知的世界。','3','2024-11-15',2,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyd45dba65-7bfe-4209-a635-33ea489e88ee'),(14,'15510845541','QazWSxEdCrFvTgbNhy!','朱思洋',NULL,'421365987@qq.com','知足常乐，随遇而安。','3','2024-11-18',1,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy32f735c7-d8b8-4d1c-b3a4-14f9232021e2'),(15,'16113049586','1q2w3e4r5t6y7u8i9o0p@','李柯雨','2003-05-01','895012365@qq.com',NULL,'1','2024-11-18',1,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyb50ca9ee-c085-4598-afcb-2ae1e03ebfbf'),(16,'18671458233','123456','史非','2004-08-25','seanfelix@whu.edu.cn',NULL,'1','2024-11-18',0,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy5484aa7c-7ba8-4952-a6eb-3c1cf26b41fb');
/*!40000 ALTER TABLE `同学` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `同学小聚关联表`
--

DROP TABLE IF EXISTS `同学小聚关联表`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `同学小聚关联表` (
  `同学ID` int NOT NULL,
  `小聚ID` int NOT NULL,
  PRIMARY KEY (`同学ID`,`小聚ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='某同学未读的某小聚存在，已读删除';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `同学小聚关联表`
--

LOCK TABLES `同学小聚关联表` WRITE;
/*!40000 ALTER TABLE `同学小聚关联表` DISABLE KEYS */;
INSERT INTO `同学小聚关联表` VALUES (2,3),(7,1),(7,3),(8,1),(8,7),(8,8),(9,4),(9,6),(13,2),(15,5),(16,9),(16,10),(16,11);
/*!40000 ALTER TABLE `同学小聚关联表` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `同学相册关联表`
--

DROP TABLE IF EXISTS `同学相册关联表`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `同学相册关联表` (
  `照片ID` int NOT NULL,
  `同学ID` int NOT NULL,
  PRIMARY KEY (`照片ID`,`同学ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='某同学与某照片间存在关联关系则存这条数据，没有则无';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `同学相册关联表`
--

LOCK TABLES `同学相册关联表` WRITE;
/*!40000 ALTER TABLE `同学相册关联表` DISABLE KEYS */;
INSERT INTO `同学相册关联表` VALUES (11,8),(13,8),(16,8),(20,8);
/*!40000 ALTER TABLE `同学相册关联表` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `回忆`
--

DROP TABLE IF EXISTS `回忆`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `回忆` (
  `回忆ID` int NOT NULL AUTO_INCREMENT COMMENT '系统分配，隐',
  `回忆主题` int DEFAULT NULL COMMENT '用int数据类型存主题的索引编号',
  `音乐URL` varchar(100) DEFAULT NULL COMMENT '云存储',
  `照片ID列表` varchar(100) DEFAULT NULL COMMENT '空格分割',
  `班级ID` int NOT NULL,
  PRIMARY KEY (`回忆ID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='【简单生成】\r\n时间分段、地点聚类、小聚ID分类\r\n【复杂生成】\r\n描述句子的NLP';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `回忆`
--

LOCK TABLES `回忆` WRITE;
/*!40000 ALTER TABLE `回忆` DISABLE KEYS */;
/*!40000 ALTER TABLE `回忆` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `小聚厅`
--

DROP TABLE IF EXISTS `小聚厅`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `小聚厅` (
  `小聚ID` int NOT NULL COMMENT '系统分配，隐',
  `班级ID` int NOT NULL,
  `发起同学ID` int NOT NULL COMMENT '判断是否为当前同学，以颜色标记',
  `时间` datetime NOT NULL COMMENT '与当前时间比较，结束的以灰色标记',
  `地点` varchar(100) DEFAULT NULL COMMENT '按地图定位需求存储，数据类型暂定',
  `描述` varchar(100) DEFAULT NULL,
  `加入同学ID列表` varchar(100) DEFAULT NULL COMMENT '空格分割，读表，判断是否为当前同学，以颜色标记',
  PRIMARY KEY (`小聚ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `小聚厅`
--

LOCK TABLES `小聚厅` WRITE;
/*!40000 ALTER TABLE `小聚厅` DISABLE KEYS */;
INSERT INTO `小聚厅` VALUES (1,1,1,'2024-11-16 09:00:00','湖北省汉阳区晴川大道南岸嘴沿江','大家一起在南岸嘴江滩公园散步','1 3 6 15'),(2,3,9,'2024-10-30 12:00:00','湖北省武昌区彭刘杨路232号','老同学们中午在艳阳天大酒店一起吃个饭','9 5 14 12'),(3,2,5,'2024-11-20 10:00:00','湖北省江汉区江汉路步行街','一起来江汉路逛街','5 4 9 10 12'),(4,3,12,'2025-01-01 14:00:00','湖北省武昌区珞珈山16号武汉大学','回计算机学院参观校友之家','12 5 14 13 11'),(5,1,3,'2024-12-29 17:00:00','湖北省洪山区后欢乐大道与杨园南路交汇处正堂时代商业2号1层11-13号','大家来老村长饭店恰个饭','3 6 7 8 16'),(6,2,7,'2025-01-05 20:00:00','湖北省江岸区江滩大道1号','来汉口江滩看夜景','7 10 12'),(7,1,8,'2024-11-19 07:07:00','武汉市武汉大学','20年后再相会','8 16'),(8,1,8,'2024-11-21 07:11:00','武汉市华中科技大学','21年后我们去隔壁相会','8 16'),(9,1,16,'2024-11-24 02:30:00','武汉市武汉大学信息学部大学生活动中心','一起来看晚会','16'),(10,1,16,'2024-11-24 19:31:00','武汉市武汉大学桂园操场','一起来看路演吧！','16');
/*!40000 ALTER TABLE `小聚厅` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `班级`
--

DROP TABLE IF EXISTS `班级`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `班级` (
  `班级ID` int NOT NULL AUTO_INCREMENT COMMENT '系统分配，隐',
  `班级名称` varchar(100) NOT NULL,
  `班级成立时间` datetime NOT NULL,
  `班长ID` int NOT NULL,
  `同学ID列表` varchar(100) DEFAULT NULL COMMENT '空格分割',
  `小聚ID列表` varchar(100) DEFAULT NULL COMMENT '空格分割',
  PRIMARY KEY (`班级ID`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `班级`
--

LOCK TABLES `班级` WRITE;
/*!40000 ALTER TABLE `班级` DISABLE KEYS */;
INSERT INTO `班级` VALUES (1,'实验中学一班','2024-06-22 00:00:00',1,'1 7 8 15 16','1 5 7 8 9 10 10'),(2,'实验小学三班','2024-06-27 00:00:00',5,'1 2 4 5 7 9 10 12 16','3 6'),(3,'实验高中六班','2024-06-10 00:00:00',9,'5 9 11 12 13 14','2 4');
/*!40000 ALTER TABLE `班级` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `班级申请表`
--

DROP TABLE IF EXISTS `班级申请表`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `班级申请表` (
  `同学ID` int NOT NULL,
  `班级ID` int NOT NULL,
  `班长ID` int NOT NULL,
  PRIMARY KEY (`同学ID`,`班级ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='处理完就删掉';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `班级申请表`
--

LOCK TABLES `班级申请表` WRITE;
/*!40000 ALTER TABLE `班级申请表` DISABLE KEYS */;
/*!40000 ALTER TABLE `班级申请表` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `班级留言`
--

DROP TABLE IF EXISTS `班级留言`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `班级留言` (
  `班级留言ID` int NOT NULL AUTO_INCREMENT COMMENT '系统分配，隐',
  `班级ID` int NOT NULL,
  `留言同学ID` int NOT NULL,
  `留言信息` varchar(100) DEFAULT NULL,
  `点赞数` int NOT NULL DEFAULT '0' COMMENT '赞越多越大，初始值为0',
  PRIMARY KEY (`班级留言ID`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `班级留言`
--

LOCK TABLES `班级留言` WRITE;
/*!40000 ALTER TABLE `班级留言` DISABLE KEYS */;
INSERT INTO `班级留言` VALUES (1,3,13,'毕业季，我们收获了知识，也收获了友谊。愿我们在未来的道路上，不忘初心，砥砺前行，共创美好未来！',22),(2,1,15,'亲爱的同学们，转眼间我们就要毕业了，感谢这四年的陪伴和友谊，祝愿大家前程似锦！',10),(3,3,14,'毕业不是结束，而是新的开始。希望我们都能在未来的道路上，勇往直前，实现自己的梦想！',15),(4,3,5,'班级里的每一个瞬间都值得珍藏，感谢大家在这四年里给予的支持和帮助，期待未来再相聚！',13),(5,2,9,'毕业季，告别校园，但我们的友谊长存。愿我们在未来的日子里，都能找到自己的方向，勇攀高峰！',23),(6,1,6,'感谢老师们的辛勤教导，让我们在知识的海洋里畅游。祝愿老师们身体健康，工作顺利！',27),(7,2,5,'时光荏苒，四年时光转瞬即逝。感谢同学们的陪伴，让我们在成长的道路上不再孤单。愿大家前程似锦，未来可期！',77),(8,1,8,'第一次使用这个系统，感觉好好用！',1),(9,1,-1,'',0),(10,1,-1,'',0),(11,1,-1,'',0),(12,1,-1,'',0),(13,1,-1,'',0),(14,1,-1,'20年后再相会',0),(15,1,-1,'20年后再相会',0),(16,1,-1,'20年后再相会',0),(17,1,16,'终于结束所有专必的学习啦！',1),(18,1,-1,'美丽的风景',0),(19,1,16,'最后在校的一年啦',0),(20,1,-1,'一起来看路演吧！',0),(21,1,-1,'一起来看晚会',0);
/*!40000 ALTER TABLE `班级留言` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `用户统计量`
--

DROP TABLE IF EXISTS `用户统计量`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `用户统计量` (
  `日期` date NOT NULL,
  `男性数量` int DEFAULT '0',
  `女性数量` int DEFAULT '0',
  `未知数量` int DEFAULT '0',
  `当天用户登录数量` int DEFAULT '0',
  `其他数量` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`日期`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `用户统计量`
--

LOCK TABLES `用户统计量` WRITE;
/*!40000 ALTER TABLE `用户统计量` DISABLE KEYS */;
INSERT INTO `用户统计量` VALUES ('2024-11-12',0,2,3,7,2),('2024-11-13',1,1,0,5,3),('2024-11-14',1,2,1,4,0),('2024-11-15',2,3,1,6,1),('2024-11-16',0,1,2,3,0),('2024-11-17',0,0,2,4,2),('2024-11-18',3,3,2,10,2);
/*!40000 ALTER TABLE `用户统计量` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `留言回复`
--

DROP TABLE IF EXISTS `留言回复`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `留言回复` (
  `留言ID` int NOT NULL AUTO_INCREMENT,
  `回复留言ID` int NOT NULL,
  `留言同学ID` int NOT NULL,
  `留言信息` varchar(100) NOT NULL,
  PRIMARY KEY (`留言ID`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `留言回复`
--

LOCK TABLES `留言回复` WRITE;
/*!40000 ALTER TABLE `留言回复` DISABLE KEYS */;
INSERT INTO `留言回复` VALUES (1,1,1,'最近过的很好呀'),(2,10,5,'不客气，生活是美好的，一定要笑面生活噢😀'),(3,3,1,'谢谢！你也是大美女呀！'),(4,9,8,'谢谢ε٩(๑> ₃ <)۶ з'),(5,8,8,'好呀，有空出来喝一杯'),(6,2,15,'我也很怀念！'),(7,7,13,'好卷啊现在还在学习（doge'),(8,1,6,'大家下次约一下！'),(9,7,14,'不客气，多多交流'),(10,10,9,'有什么不开心的也可以找我倾诉！'),(11,6,7,'好的，我也很期待下次一起聚。'),(12,5,5,'一定的一定的，我周末准时到！'),(13,2,3,'再约着一起打篮球吧'),(14,4,10,'谢谢谢谢，也祝你一切顺利'),(15,5,4,'好期待呀！好久不见了'),(16,4,10,'你现在在哪高就啊');
/*!40000 ALTER TABLE `留言回复` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `相册`
--

DROP TABLE IF EXISTS `相册`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `相册` (
  `照片ID` int NOT NULL AUTO_INCREMENT COMMENT '系统分配，隐',
  `图片URL` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '云存储',
  `班级ID` int NOT NULL,
  `上传同学ID` int NOT NULL,
  `小聚ID` int DEFAULT NULL COMMENT '空则照片不来自小聚，可用作tag',
  `时间` datetime NOT NULL COMMENT '可用作tag',
  `地点` varchar(100) DEFAULT NULL COMMENT '可用作tag',
  `描述` varchar(100) DEFAULT NULL,
  `点赞数` int NOT NULL DEFAULT '0' COMMENT '初始值为0，赞越多越大',
  PRIMARY KEY (`照片ID`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `相册`
--

LOCK TABLES `相册` WRITE;
/*!40000 ALTER TABLE `相册` DISABLE KEYS */;
INSERT INTO `相册` VALUES (1,'no-photo',3,-1,NULL,'2024-07-15 00:00:00','','',0),(2,'no-photo',1,-1,NULL,'2024-10-23 00:00:00','','',0),(3,'no-photo',3,-1,NULL,'2024-06-30 00:00:00','','',0),(4,'no-photo',3,-1,NULL,'2024-08-17 00:00:00','','',0),(5,'no-photo',2,-1,NULL,'2024-08-21 00:00:00','','',0),(6,'no-photo',1,-1,NULL,'2024-09-04 00:00:00','','',0),(7,'no-photo',2,-1,NULL,'2024-09-30 00:00:00','','',0),(8,'no-photo',1,-1,NULL,'2024-11-17 23:05:38','第一次使用这个系统，感觉好好用！','第一次使用这个系统，感觉好好用！',0),(9,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy32f735c7-d8b8-4d1c-b3a4-14f9232021e2',1,16,7,'2024-11-18 09:36:22','','',0),(11,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy3cb1cb2a-c1c3-4f5a-8dc4-abbb157e15c4',1,16,1,'2024-11-18 09:55:44','','',0),(12,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyee42eedd-b261-4e01-b5e3-388191d61fa0',1,16,7,'2024-11-18 09:57:06','','',0),(13,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye4c6e922-7c84-42be-933b-b8ba5a603db0',1,16,7,'2024-11-18 09:59:49','','',1),(14,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcya99a4210-4252-4a14-affe-665db3ab15f7',1,16,7,'2024-11-18 10:21:35','武汉市武汉大学','20年后再相会',0),(15,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcy1c678fd1-ef02-413e-98af-78db33e7fcab',1,16,7,'2024-11-18 10:29:43','武汉市武汉大学','20年后再相会',0),(16,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcyd45dba65-7bfe-4209-a635-33ea489e88ee',1,16,7,'2024-11-18 11:26:58','武汉市武汉大学','20年后再相会',1),(17,'no-photo',1,-1,NULL,'2024-11-18 11:31:46','终于结束所有专必的学习啦！','终于结束所有专必的学习啦！',0),(18,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye4b40d0f-194d-41d4-bed4-1e1b77d6aed3',1,16,NULL,'2024-11-18 11:32:57','美丽的风景','美丽的风景',1),(19,'no-photo',1,-1,NULL,'2024-11-18 12:24:22','最后在校的一年啦','最后在校的一年啦',0),(20,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcybaad2e9e-9ece-478f-b224-2fae89005d05',1,16,10,'2024-11-18 12:32:24','武汉市武汉大学桂园操场','一起来看路演吧！',1),(21,'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/lcye00c4d0f-8c4a-4974-bc2d-2d81dba14274',1,16,9,'2024-11-18 13:33:10','武汉市武汉大学信息学部大学生活动中心','一起来看晚会',0);
/*!40000 ALTER TABLE `相册` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'classmate_trace'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-18 15:26:46
