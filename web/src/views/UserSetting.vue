<template>
    <div class="usct-index">
        <top-bar></top-bar>
        <video src="@/assets/images/bg1.mp4" autoplay muted loop class="bg-video"></video>
        <div class="settings-container">
            <!-- <img :src="userProfile.avatar" class="user-avatar" :class="{ editable: editable }" @click="editable && uploadAvatar()" /> -->
            <img :src="userProfile.avatar" class="user-avatar" :class="{ editable: editable }" @click="editable && uploadAvatar()" />
            <input type="file" ref="avatarInput" style="display: none;" @change="handleAvatarChange" />


            <div class="form">
                <div class="form-item-horizontal">
                    <span class="user-phone">{{ userProfile.phone }}</span>
                </div>
                <div class="form-item-horizontal">
                    <div class="form-item-half">
                        <label>姓名:</label>
                        <input type="text" v-model="userProfile.name" placeholder="姓名" :disabled="!editable" />
                    </div>
                    <div class="form-item-half">
                        <label>性别:</label>
                        <select v-model="userProfile.gender" :disabled="!editable">
                            <option value="0">男</option>
                            <option value="1">女</option>
                            <option value="2">其他</option>
                            <option value="3">未知</option>
                        </select>
                    </div>
                </div>
                <div class="form-item-horizontal">
                    <label>密码:</label>
                    <input type="password" v-model="userProfile.password" placeholder="密码" :disabled="!editable" />
                </div>
                <div class="form-item-horizontal">
                    <label>邮箱:</label>
                    <input type="email" v-model="userProfile.email" placeholder="未绑定" :disabled="!editable" />
                </div>
                <div class="form-item-horizontal">
                    <div class="form-item-half">
                        <label>生日:</label>
                        <input type="date" v-model="userProfile.birthday" :disabled="!editable" />
                    </div>
                    <!--
                    <div class="form-item-half">
                        <label>位置:</label>
                        <ct-buttons-vue nom="c_secondary" class="location-btn" @click="selectLocation">选择位置</ct-buttons-vue>
                    </div>
                    -->
                </div>
                <ct-buttons-vue nom="c_primary" class="update-btn" @click="toggleEditable">{{ editable ? '确认修改' : '修改信息' }}</ct-buttons-vue>
            </div>
        </div>
        
        <foot-bar-vue></foot-bar-vue>
    </div>
</template>

<script>
import { settingApi, personalApi } from '@/api/index'
import topBar from '@/components/TopBar.vue';
import footBarVue from '@/components/FootBar.vue';
import CtButtonsVue from '@/components/CTButtons.vue'; // 引用按钮组件

// import COS from 'cos-js-sdk-v5';
import OSS from 'ali-oss';
import { v4 as uuidv4 } from 'uuid';


export default {
    data() {
        
        return {
            files: '',
            editable: false, // 控制是否可编辑状态
            userProfile: {
                avatar: 'http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/05315455-bd55-4097-b2fc-c5e189e9b721', // 默认头像路径
                name: 'HeiHei',
                phone: '电话: 123-456-7890',  // 示例电话，这里你可以设置为实际用户电话数据
                email: '666@163.com',
                password: '77777',
                birthday: '2001-01-01',
                gender: 4, // 默认性别为未知
                signature: ''
            }
        };
    },
    created(){
        this.fetchUserData();
    },
    computed:{
        userID()
        {
            return this.$store.state.user.id;
        },
        classID()
        {
            return this.$store.state.classId;
        },
    },
    components: {
        topBar,
        footBarVue,
        CtButtonsVue,
    },
    methods: {
        async editPersonalInfo() {
            
            try{
            const infoData = {
                sign: this.userProfile.signature,
                classmate_id: this.userID,
                username: this.userProfile.name,
                birthdayString: this.userProfile.birthday,
                mail: this.userProfile.email,
                gender: this.userProfile.gender,
                headPicture: this.userProfile.avatar
            };
            console.log("@$^&%$@(&@@$&(%%%%%%%%%%$$$$$$$$$###########))")
            const response = await personalApi.updatePersonalInfo(infoData);
            console.log('####Personal info edit successful:', response);
            // Handle success response here
            }catch(error){
            console.error('Error editting personal info:', error);
            // Handle error here
            }
      },
        async fetchUserData(){
            try{
                console.log("正在查询");
                console.log(this.userID)
                const response = await settingApi.getUserInfo(this.userID);
                console.log('查询得到信息:', response);
                const userData = response;
                this.userProfile.avatar = userData.headPicture;
                this.userProfile.signature = userData.sign;
                this.userProfile.name = userData.name;
                this.userProfile.email = userData.mail;
                this.userProfile.password = userData.password;
                if (userData && userData.birthday) {
                    this.userProfile.birthday = userData.birthday.substring(0, 10);
                } else {
                    this.userProfile.birthday=null;
                }
                this.userProfile.gender = userData.gender;
                this.userProfile.phone="电话："+userData.phoneNumber;
                console.log("查询成功");
                console.log(userData);
            }catch(error){
                console.error('Failed to fetch user data:', error);
            }
        },
        uploadAvatar() {
            if (this.editable) {
                this.$refs.avatarInput.click();
            }
        },
        async handleAvatarChange(event) {
            const file = event.target.files[0];
            
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.userProfile.avatar = e.target.result;
                    // console.log(e.target.result);
                };
            await reader.readAsDataURL(file);
            
            const avatarUrl = await this.onUploadFile(file);
            console.log("@@@@@@@@@@@@@@@@新头像的url：",avatarUrl);
            this.userProfile.avatar = avatarUrl;
            const response = this.editPersonalInfo();
            console.log(response, "######", avatarUrl);
        }
         

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
        selectLocation() {
            if (this.editable) {
                // 处理位置选择逻辑
                console.log('位置选择');
            }
        },
        async toggleEditable() {
            try{
                if (this.editable) {
                    // 处理更新信息逻辑
                    console.log('用户信息:', this.userProfile);
                    if(this.userProfile.name=='')
                    {
                        alert("姓名不可以为空！");
                        this.fetchUserData();
                        this.editable = !this.editable;
                        return ;
                    }
                    if(this.userProfile.password=='')
                    {
                        alert("密码不可以为空！");
                        this.fetchUserData();
                        this.editable = !this.editable;
                        return ;
                    }
                    //这里还需要往服务器发送图片，然后获得一个url，再把url给存到后端去
                    const data = {
                        sign: this.userProfile.signature,
                        classmate_id: this.userID,
                        password: this.userProfile.password,
                        username: this.userProfile.name,
                        birthdayString: this.userProfile.birthday,
                        mail: this.userProfile.email,
                        gender: this.userProfile.gender,
                        headPicture: this.userProfile.avatar,
                        // addressInfo: 'Your address information here' // Add address information here
                    }
                    console.log(data)
                    const response = await settingApi.updateUserInfo(data);
                    console.log("成功修改信息",response); // 成功时的返回数据
                    alert("修改信息成功！");
                }
                this.editable = !this.editable;
            }catch(error){
                console.error("又你妈出错了",error.response.data); // 处理请求错误
            }           
        }
    }
};
</script>

<style lang="less" scoped>
.usct-index {
    position: relative;
    height: 100vh;
    display: flex;
    flex-direction: column;
    align-items: center;

    .bg-video {
        position: fixed;
        top: 0;
        left: 0;
        height: "840";
        object-fit: cover;
        z-index: -1;
    }

    .settings-container {
        width: 60%;
        max-width: 700px;
        margin-top: 50px;
        padding: 40px;
        border: 1px solid #ddd;
        border-radius: 10px;
        box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
        backdrop-filter: blur(10px);
        // background-color: rgba(255, 255, 255, 0.9);
        z-index: 1;
        display: flex;
        flex-direction: column;
        align-items: center;

        .user-avatar {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            margin-bottom: 20px;
            cursor: pointer;
            &.editable {
                border: 2px dashed #abd8ff; // 添加虚线边框表示可编辑状态
            }
        }

        .form {
            display: flex;
            flex-direction: column;
            width: 100%;
            align-items: center;

            .form-item-horizontal {
                display: flex;
                flex-direction: row;
                align-items: center;
                width: 100%;
                margin-bottom: 20px;

                label {
                    margin-right: 10px;
                    margin-left: 10px;
                    font-size: 16px;
                    color: #333;
                }

                input, select, .user-phone {
                    margin-right: 10px;
                    flex: 1;
                    padding: 10px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    font-size: 16px;
                    box-sizing: border-box;
                }

                .user-phone {
                    text-align: center;
                    border: none;
                }
            }

            .form-item-half {
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: space-between;
                width: 60%;
                margin-bottom: 0;

                label {
                    margin-right: 10px;
                    margin-left: 10px;
                }

                input, select {
                    margin-right: 10px;
                    flex: 1;
                    padding: 10px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    font-size: 16px;
                    box-sizing: border-box;
                }
            }

            .location-btn, .update-btn {
                padding: 8px;
                font-size: 16px;
                width: 80%;
            }

            .location-btn {
                margin-bottom: 5px;
                background-color: rgba(255,255,255,0.5);
            }
        }
    }
}
</style>
