<template>
    <div class="ct-index">
        <img src="@/assets/images/classmaps_logo_main.png" class="logo-img"/>
        <video src="@/assets/images/bg1.mp4" autoplay="autoplay" muted="muted" loop="loop" class="bg-video"></video>
        <div class="auth-container">
            
            <div class="tabs">
                <p class="tab-item" :class="{selected: isLogin}" @click="isLogin = true">登录</p>
                <p class="tab-item" :class="{selected: !isLogin}" @click="isLogin = false">注册</p>
            </div>
            <div class="form" v-if="isLogin">
                
                <input type="text" v-model="loginData.phonenumber" placeholder="电话号码" />
                <input type="password" v-model="loginData.password" placeholder="密码" />
                <ct-buttons-vue nom="c_primary" class="menu1" @click="login">登录</ct-buttons-vue>
            </div>
            <div class="form" v-else>
                
                <input type="text" v-model="registerData.username" placeholder="用户名" />
                <input type="text" v-model="registerData.phonenumber" placeholder="电话号码" />
                <input type="password" v-model="registerData.password" placeholder="密码" />
                <ct-buttons-vue nom="c_primary" class="menu1" @click="register">注册</ct-buttons-vue>
            </div>
        </div>
        
    </div>
</template>

<script>

import CtButtonsVue from '@/components/CTButtons.vue' // 引用buttons文件
import { loginregisterApi, adminApi } from '@/api/index'
// import CTLoginService from "@/services/CTLoginService.ts"
// import RegisterService from "@/services/CTRegisterService.ts"
// import LoginStatus from "@/types/LoginStatus.tsx"
// import Classmate from "@/types/Classmate.ts"
export default {
    data() {
        return {
            isLogin: true, // 用于切换登录和注册
            userId: Number,
            loginData: {
                phonenumber: '',
                password: ''
            },
            registerData: {
                username: '',
                phonenumber: '',
                password: '',
            }
        };
    },
    components: {
        CtButtonsVue,
    },
    methods: {
        clearLoginData(){
            this.loginData.phonenumber = '';
            this.loginData.password = ''; // 输入栏清空
        },
        clearRegisterData(){
            this.registerData.username = '';
            this.registerData.phonenumber = '';
            this.registerData.password = ''; // 输入栏清空
        },
        /*
        switchToMain() {
            this.$router.push('/map')
        },*/
        /*
        switchToAdmin() {
            this.$router.push('/admin')
        },*/
        async updateInfo(){
            try{
                //更新用户统计量
                const response = await adminApi.updateUserStats();
                console.log('用户统计量更新成功:', response.data);
            }catch(e){
                console.log('用户统计量更新失败:', e.response ? e.response.data : e.message);
            }
        },      
        async login() {           
            try{
                // 在这里处理登录逻辑
                console.log('登录信息:', this.loginData);
                if(this.loginData.phonenumber == 'admin' && this.loginData.password == 'admin'){
                    console.log('后台管理员登录成功');
                    // 跳转至后台管理页面AdminInterface.vue
                    this.$store.commit('setLoginStatus', true);
                    this.$router.push('/admin');
                    this.$message({ type: "success", message: "登录成功！" });
                    return;
                }
                const params = {
                    phonenumber: this.loginData.phonenumber,
                    password: this.loginData.password
                }
                const response = await loginregisterApi.loginUser(params);
                // console.log('应答:', response);         
                // 用户不存在
                if(response.exist == false){
                    console.log('登录失败:', response.message);
                    // 错误消息弹窗显示message内容
                    // 输入栏清空
                    // alert(response.message); // 错误消息弹窗显示message内容
                    this.$message({ type: "error", message: "登录失败。" });
                    this.clearLoginData();
                    return;
                }
                // 密码错误
                if(response.message == '密码错误'){
                    console.log('登录失败:', response.message);
                    // 错误消息弹窗显示message内容
                    // 输入栏清空
                    alert(response.message); // 错误消息弹窗显示message内容
                    this.clearLoginData();
                    return;
                }              
                // handle success logic
                console.log('登录成功:', response);
                this.$message({ type: "success", message: "登录成功！" });
                this.userId = response.userId;
                // 输入栏清空
                this.clearLoginData();          
                // 更新用户统计量   
                this.updateInfo();            
                // 页面跳转至主页并传入用户Id(this.userId)
                let user = {
                    id: this.userId,
                }
                this.$store.commit('getUser', user);
                this.$store.commit('setLoginStatus', true);
                this.$router.push('/map'); // 跳转到主页面
                // this.switchToMain();
            }
            catch(e){
                console.log('登录失败:', e.response ? e.response.data : e.message);
                // handle error logic
                // 错误消息弹窗显示e内容
                // 输入栏清空
                // alert(e.message); // 错误消息弹窗显示e内容
                this.$message({ type: "error", message: "登录失败。" });
                this.clearLoginData();
                return;
            }           
        },
        async register() {
            try{
                // 在这里处理注册逻辑
                console.log('注册信息:', this.registerData);
                if (this.registerData.password == '' || this.registerData.username == '' || this.registerData.phonenumber == '') {
                    console.log('基本信息为空');
                    // 提醒消息弹窗
                    alert('请填写所有必填信息'); // 提醒消息弹窗
                    return;
                }
                // this.axios.post('https://localhost:7162/entrance/register', data)
                // this.axios.post('https://localhost:7162/entrance/register', JSON.stringify(data) ,{headers:{'Content-Type':'application/json; charset=UTF-8'}})
                const data = {
                    username: this.registerData.username,
                    password: this.registerData.password,
                    phonenumber: this.registerData.phonenumber,
                    gender: 3,  // 注册时默认性别为未知
                    mail: '',
                    sign: '',
                    headpicture: '',
                    birthdayString: null
                }
                const response = await loginregisterApi.registerUser(data);    
                if(response.state == false){
                    // alert('注册失败: 号码' + this.registerData.phonenumber + '已存在');
                    this.$message({ type: "error", message: "注册失败：号码"+this.registerData.phonenumber+"已存在" });
                    // this.userId = response.ClassmateId;
                    this.clearRegisterData();
                    return;
                }
                console.log('注册成功:', response);
                this.$message({ type: "success", message: "注册成功！" });
                // handle success logic
                this.userId = response.classmate.classmateId;
                this.clearRegisterData();
                // 更新用户统计量   
                this.updateInfo(); 
                // 页面跳转至主页并传入用户Id(this.userId)
                // this.switchToMain();
                // 页面跳转至主页并传入用户Id(this.userId)
                let user = {
                    id: this.userId,
                }
                this.$store.commit('getUser', user);
                this.$store.commit('setLoginStatus', true);
                this.$router.push('/map'); // 跳转到主页面
            }catch(e){
                console.log('注册失败:', e);
                // handle error logic
                // 错误消息弹窗显示e内容
                // 输入栏清空
                // alert(e.message); // 错误消息弹窗显示e内容
                this.$message({ type: "error", message: "注册失败。" });
                this.clearRegisterData();
            }
        }
    }
};
</script>

<style lang="less" scoped>
.ct-index {
    position: relative;
    height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    align-items: center;

    .bg-video {
        position: fixed;
        top: 0;
        left: 0;
        height: "840";
        object-fit: cover;
        z-index: -1;
    }

    .auth-container {
        position: fixed;
        top: 32%; /* 固定在页面上边位置 */
        left: 50%;
        width: 40%;
        transform: translateX(-50%);
        max-width: 400px;
        margin: auto;
        padding: 40px;
        border: 1px solid #ddd;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        
        backdrop-filter: blur(10px); // 毛玻璃效果
        z-index: 1;
        text-align: center;

        

        .tabs {
            display: flex;
            justify-content: space-around;
            margin-bottom: 20px;

            .tab-item {
                cursor: pointer;
                padding: 10px 20px;
                font-size: 24px; /* 调整字体大小 */
                border-bottom: 2px solid transparent;
            }

            .tab-item.selected {
                border-bottom: 2px solid @primary-color;
                font-weight: bold;
            }
        }

        
        .form {
            display: flex;
            flex-direction: column;

            input {
                margin-bottom: 15px;
                padding: 12px;
                border: 1px solid #ddd;
                border-radius: 5px;
                font-size: 14px;
            }


        }
    }
    .logo-img {
        position: absolute; /* 绝对定位 */
        top: 45px; /* 调整图片位置 */
        width: 180px; /* 缩小logo */
        filter: grayscale(5%); /* 灰色 */
        z-index: 1;
    }
}
</style>
