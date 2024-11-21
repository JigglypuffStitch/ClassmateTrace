<template>
    <div class="new-card">
        <div class="colors" v-show="id==0"> <!--id=0才显示颜色选择-->
            <p class="color-li" v-for="(e,index) in cardColor1" :key="index" :style="{background: e}" :class="{colorselected: index==colorSelected}" @click="changeColor(index)"></p><!--读颜色-->
        </div>
        <!--照片-->
        <div class="add-photo" v-if="id==1">
            <input type="file" name="file" id="file" multiple="multiple" ref="avatarInput" @change="showPhoto" /> <!--上传图片时触发的事件-->
            <div class="add-bt" v-if="url==''">
                <span class="iconfont icon-tianjia"></span> <!--顶层的伪按钮-->
            </div>
            <div class="change-bt" v-if="url!=''">
                <span class="cticon icon-xiugai"></span> <!--url非空时可以修改-->
            </div>
            <div class="photo-div"><img :src="url"/></div> <!--渲染图片-->
        </div>
        <!--卡片-->
        <div class="card-main" :style="{background: id==0?cardColor[colorSelected]:cardColor[5]}">
            <textarea placeholder="在此处留言..." class="message" maxlength="96" v-model="message"></textarea> <!--最多字符96个-->
            <input type="text" placeholder="签名" class="name" v-model="name"/>
        </div>
        <div class="labels">
            <p class="title">选择标签</p> <!--留言和照片共用，所以直接从页面读id并传-->
            <div class="label">
                <p v-for="(e,index) in label[id]" :key="index" class="label-li" :class="{labelselected: index==labelSelected}" @click="changeLabel(index)">{{ e }}</p>
            </div>
        </div>
        <div>
            <p class="title">留言提醒</p>
            <p class="notification">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;使用同窗行迹留言墙时，请您保持文明和礼貌，不发表与主题无关的言论，不得使用侮辱性、挑衅性的语言，保持客观和理性，以营造良好的交流氛围。<br>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;请不要利用此平台服务制作、上传、下载、复制、发布、传播或者转载如下内容：<br>
                1、反对宪法所确定的基本原则的；<br>
                2、危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家 统一的；<br>
                3、损害国家荣誉和利益的；<br>
                4、煽动民族仇恨、民族歧视，破坏民族团结的；<br>
                5、破坏国家宗教政策，宣扬邪教和封建迷信的；<br>
                6、散布谣言，扰乱社会秩序，破坏社会稳定的；<br>
                7、散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；<br>
                8、侮辱或者诽谤他人，侵害他人合法权益的；<br>
                9、含有法律、行政法规禁止的其他内容的信息。
            </p>
        </div>
        <div class="footbt">
            <ct-buttons-vue size="max" nom="secondary" @click="closeModal(0)">丢弃</ct-buttons-vue>
            <ct-buttons-vue size="max" class="submit" @click="submit()">确定</ct-buttons-vue>
        </div>
    </div>
</template>

<script>
import {cardColor, cardColor1, label} from "@/utils/data"
import ctButtonsVue from "./CTButtons.vue"
import { getObjectURL } from "@/utils/methods"
import { insertWallApi, commentApi, albumApi } from "@/api/index"

// import COS from 'cos-js-sdk-v5';
import OSS from 'ali-oss';
import { v4 as uuidv4 } from 'uuid';

export default {
    data() {
        return {  // 变量
            cardColor,
            cardColor1,
            label,
            colorSelected: 0, // 当前选择颜色
            labelSelected: 0, // 当前选择标签
            message: '', // 留言信息
            name: '', // 签名
            user: this.$store.state.user, 
            url: '', // 图片地址
            classId: this.$store.state.classId,
            pfile: null,
        }
    },
    props: {
        id: {  // 去父级页面拿，当前在留言墙还是照片墙
            default: 0,
        }
    },
    components: {
        ctButtonsVue,
    },
    methods: {
        changeColor(e) { // 点击切换颜色
            this.colorSelected = e;
        },
        changeLabel(e) { // 点击切换标签
            this.labelSelected = e;
        },
        closeModal(data) { // 丢弃（关闭窗口），data可以用来封装返回信息，用于确定的时候传信息
            this.$emit('addClose', data);
        },
        submit() { // 正式的提交函数
            let name = '匿名';
            if (this.name) {  // 签名可匿名
                name = this.name;  // 有输入时就是输入的内容
            }
            // 创建要提供给后端的数据项
            let data = {
                type: this.id, // 留言/照片
                message: this.message, 
                name: name,
                userId: this.user.id,
                moment: new Date(),
                label: this.labelSelected,
                color: 5,
                imgurl: '',  // 图片会在uploadPhoto(data)赋值给它
                classId: this.classId,
            };
            // console.log(data)
            // 留言判断
            if (this.message && this.id == 0) {
                    data.color = this.colorSelected, // 留言才记录color的修改
                    insertWallApi(data).then((res) => {  // 后端提交
                        // console.log(res); // 先验证
                        let cardD = {
                            type: this.id, // 留言/照片
                            message: this.message, 
                            name: name,
                            userId: this.user.id,
                            moment: new Date(),
                            label: this.labelSelected,
                            color: this.colorSelected,
                            imgurl: '',
                            id: res.message.insertId,  // 可以console.log看到res中有id！
                            islike: [{count:0}],
                            like: [{count:0}],
                            comcount: [{count: 0}],
                            report: [{count: 0}],
                            revoke: [{count: 0}],
                            classId: this.classId,
                        }
                        this.message = '';  // 清空消息
                        this.$emit("clickbt", cardD);  // 告诉父级
                        this.$message({ type: "success", message: "感谢你的留言！" });
                        if (cardD.type == 0) {
                            this.insertMessage(cardD);
                        }
                    });
            }
            else if (this.id == 1 && this.url) {  // 并且还需要有url
                this.uploadPhoto(data);
            }
        },
        showPhoto(event) {
            this.pfile = event.target.files[0];
            let address = getObjectURL(document.getElementById("file").files[0]); // id去获取files的第0项 - 暂时的图片地址
            this.url = address
        },
        async onUploadFile(file) {
            if (!file) {
                return "空文件无法获取url";
            }
            const client = new OSS({
                region: '',
                accessKeyId: '',
                accessKeySecret: '',
                bucket: ''
            });

            const uuid = uuidv4();
            const directoryPath = 'lcy'; // 指定目录路径
            const objectKey = `${directoryPath}${uuid}`; // 将路径与文件名拼接

            const r1 = await client.put(objectKey, file); // 使用完整路径上传
            console.log('put success: %j', r1.url);
            const r2 = await client.get(objectKey);
            console.log('get success: %j', r2);

            return r1.url;
        },
        async uploadPhoto(data) {  // 图片提交
            const file = this.pfile;
            this.pfile=null;
            
            const avatarUrl = await this.onUploadFile(file);
            console.log("照片墙的新url：",avatarUrl);
            data.imgurl = avatarUrl;
            insertWallApi(data).then((result) => {  // 后端提交
                // console.log(res); // 先验证
                let cardD = {
                    type: this.id, // 留言/照片
                    message: this.message, 
                    name: data.name,
                    userId: this.user.id,
                    moment: new Date(),
                    label: this.labelSelected,
                    color: 5,
                    imgurl: avatarUrl,
                    id: result.message.insertId,  // res中有id！
                    islike: [{count:0}],
                    like: [{count:0}],
                    comcount: [{count: 0}],
                    report: [{count: 0}],
                    revoke: [{count: 0}],
                    classId: this.classId,
                }
                this.message = '';  // 清空消息
                this.$emit("clickbt", cardD);  // 告诉父级
                this.$message({ type: "success", message: "感谢你的记录！" });
                this.insertPhoto(cardD);
            });
            /*
            if (file.files.length > 0) {
                let formData = new FormData();  // 异步上传二进制文件
                formData.append('file', file.files[0]);
                // console.log(file.files[0])
                // 提交后端
                profileApi(formData).then((res) => {
                    // console.log(res)  // 后端send来的
                    // 存储到数据库
                    data.imgurl = res;  // /photo/name
                    insertWallApi(data).then((result) => {  // 后端提交
                        // console.log(res); // 先验证
                        let cardD = {
                            type: this.id, // 留言/照片
                            message: this.message, 
                            name: data.name,
                            userId: this.user.id,
                            moment: new Date(),
                            label: this.labelSelected,
                            color: 5,
                            imgurl: res,
                            id: result.message.insertId,  // res中有id！
                            islike: [{count:0}],
                            like: [{count:0}],
                            comcount: [{count: 0}],
                            report: [{count: 0}],
                            revoke: [{count: 0}],
                            classId: this.classId,
                        }
                        this.message = '';  // 清空消息
                        this.$emit("clickbt", cardD);  // 告诉父级
                        this.$message({ type: "success", message: "感谢你的记录！" });
                        this.insertPhoto(cardD);
                    });
                })
            }*/
        },
        apiTest() { // 接口测试使用，绑定到点击按钮
            let data = {
                type: 0,
                message: '测试数据',
                name: '史非',
                userId: '32', 
                moment: new Date(), 
                label: 0, 
                color: 3, 
                imgurl: 'www.baidu.com'
            }
            // axios访问
            this.axios
                .post("http://localhost:3000" + "/insertwall", data) // 对应后端的地址
                .then((res) => {
                    console.log(res)
                })
        },
        async insertMessage(cardD) {
            // 更新留言
            let data = {
                classId: cardD.classId,
                comment: cardD.message,
                userId: cardD.userId,
                // mid: cardD.id, // 暂时不硬加
            }
            // console.log(data);
            const response = await commentApi.postComment(data);
            console.log(response);
            // 更新相册
            let data_async = {
                classmate_id: -1, // cardD.userId, 
                image: "no-photo",
                classId: cardD.classId,
                dataString: null,
                address: cardD.message,
                description: cardD.message,
                // mid: cardD.id, // 暂时不硬加
            }
            const response_async = await albumApi.uploadPicture(data_async);
            console.log(response_async);

        },
        async insertPhoto(cardD) {
            // 更新相册
            let data = {
                classmate_id: cardD.userId, 
                image: cardD.imgurl,
                classId: cardD.classId,
                dataString: null,
                address: cardD.message,
                description: cardD.message,
                // mid: cardD.id, // 暂时不硬加
            }
            // console.log(data);
            const response = await albumApi.uploadPicture(data);
            console.log(response);
            // 更新留言
            let data_async = {
                classId: cardD.classId,
                comment: cardD.message,
                userId: -1,
            }
            const response_async = await commentApi.postComment(data_async);
            console.log(response_async);
        }
    },
}
</script>

<style lang="less" scoped>
.new-card {
    padding: 0 @padding-20 100px; // 120px要保证可上去
    position: relative;
    .colors {
        padding-bottom: @padding-12;
        display: flex;
        align-items: center;
        .color-li {
            width: 24px;
            height: 24px;
            margin-right: @padding-8;
            cursor: pointer;
        }
        .colorselected {
            border: 1px solid @primary-color;
        }
    }
    .card-main {
        height: 240px;
        width: 100%;
        // background: rgba(252,175,162,0.31); // 先暂时用静态处理
        padding: 12px;
        box-sizing: border-box;
        transition: @tr; // 过渡
        .message {
            background: none;
            border: none;  // 无边框
            resize: none;  // 不可变化大小
            padding: @padding-8;
            box-sizing: border-box;
            height: 172px;
            width: 100%;
            font-family: wordf;
            font-size: 15px;
        }
        .name {
            width: 100%;
            box-sizing: border-box;
            padding: @padding-8;
            background: none;
            border: @gray-4 1px solid;
            line-height: 20px;
            font-family: wordf;
            font-size: 15px;
        }
    }
    .title {  // 放外面，可以共用样式
        color: @gray-0;
        font-weight: 600;
        padding-top: 30px;
    }
    .label {
        display: flex;
        flex-wrap: wrap; // 换行
        width: 320px;
        .label-li {
            padding: 2px 10px;
            border-radius: 20px; // 圆角
            margin: 16px @padding-4 0 0;
            cursor: pointer;
            color: @gray-1;
            transition: @tr; // 过渡
        }
        .labelselected {
            background: #EBEBEB;
            font-weight: 600;
            color: @gray-0;
        }
    }
    .notification {
        padding-top: 10px;
        font-size: 12px;
        color: @gray-2;
        line-height: 27px;
    }
    .add-photo {
        padding-bottom: 20px;
        position: relative;

        #file {  // input按钮
            position: absolute;
            z-index: 10;
            top: -10px;
            height: 74px;
            width: 64px;
            opacity: 0;
            cursor: pointer;
        }

        .add-bt {
            width: 64px;
            height: 64px;
            border: 1px solid @gray-2;
            border-radius: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            .icon-tianjia {
                font-size: 24px;
            }
        }
        .photo-div {
            max-height: 200px;  // 照片只显示中间一段
            width: 100%;
            background: #f0f0f0;
            overflow: hidden;
            display: flex;
            align-items: center;
            img {
                width: 100%;
            }
        }
        .change-bt {
            position: absolute;
            top: 12px;
            left: 12px;
            height: 40px;
            width: 40px;
            border-radius: 50%;
            background: rgba(0,0,0,0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            .icon-xiugai {
                color: @gray-3;
            }
        }
    }
    .footbt {
        padding: @padding-20;
        box-sizing: border-box;
        position: fixed;
        left: 0;
        bottom: 52px;
        width: 100%;
        background: rgba(255,255,255,0.2);
        backdrop-filter: blur(20px); // 毛玻璃
        .submit {
            margin-left: @padding-20;
            width: 200px;
        }
    }
}
</style>