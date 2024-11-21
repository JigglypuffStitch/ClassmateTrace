<template>
    <div class="ps-index">
      <top-bar></top-bar>
      <video src="@/assets/images/bg1.mp4" autoplay muted loop class="bg-video"></video>

        <!-- 悬浮按钮 -->
        <button class="toggle-button" @click="toggleSidebar">
        <!-- 使用一个图标或文字来表示按钮的功能 -->
        &#9776; <!-- HTML实体字符，代表一个菜单图标 -->
        </button>

        <!-- 返回个人主页按钮 -->
        <button v-show="userID!=curID" class="return-button" @click="returnToOwnPage">返回个人主页</button>

        <!-- 侧边栏 -->
        <div class="pssidebar" v-show="isSidebarVisible">    
            <div class="pssidebar-title" >班级成员</div>
            <!-- 侧边栏内容 -->
            <!-- <button v-for="(item, index) in classmates" :key="index" class="sidebar-button">{{ item.label }}</button> -->
            <button v-for="(item, index) in classmates" :key="index" class="pssidebar-button" @click="handleClassmateClick(item)">{{ item.name }}</button>
        </div>

      <div class="user-profile">
        <div style="height: 10vh;"></div> <!-- 空行 -->
        <img :src="userProfile.avatar" class="user-avatar" :class="{ editable: editable }" @click="editable && uploadAvatar()" />
        <input type="file" ref="avatarInput" style="display: none;" @change="handleAvatarChange" />

        <!-- <div class="signature">{{ userProfile.sign }}</div> -->
        

    <div class="signature-section">
      <p>{{ signature }}</p><button v-show="curID === userID" @click="openSignatureModal" class="modify-button"> <img src="@/assets/modify.jpg" alt="Modify" />
      </button>
    </div>

     
    <SignatureModal v-if="isModalVisible" :signature="signature" @save="saveSignature" @close="closeModal" />
    <!-- <button v-show="curID === userID" @click="openSignatureModal" class="modify-button">
        <img src="@/assets/modify.jpg" alt="Modify" />
    </button> -->




        <div class="user-info">
          <div class="form-item">
            <label class="pslabel">姓名:</label>
            <span class="psspan">{{ userProfile.name }}</span>
          </div>
          <div class="form-item">
            <label class="pslabel">性别:</label>
            <span class="psspan">{{ userProfile.gender === 0 ? '男' : userProfile.gender === 1 ? '女' : userProfile.gender === 2 ? '其他' : '未知' }}</span>
          </div>
          <div class="form-item">
            <label class="pslabel">邮箱:</label>
            <span class="psspan">{{ userProfile.email }}</span>
          </div>
          <div class="form-item">
            <label class="pslabel">生日:</label>
            <span class="psspan">{{ userProfile.birthday ? userProfile.birthday.substring(0, 10) : '' }}</span>
          </div>
          <div class="form-item">
            <label class="pslabel">电话:</label>
            <span class="psspan">{{ userProfile.phone }}</span>
          </div>
        </div>
      </div>

      <div style="height: 5vh;"></div> <!-- 空行 -->
      <!-- 顶端按钮菜单栏 -->
    <div class="menu-bar">
        <div style="height: 10vh;"></div> <!-- 空行 -->
        <button @click="showPhotoWall">照片墙</button>
        <button @click="showMessageBoard">留言板</button>
    </div>

    <!-- 容器，根据当前显示的内容变化 -->
    <div class="container-with-menu">

        <!-- 照片墙 -->
        <div class="waterFall-box" v-if="currentView === 'photos'" ref="box">
            <div class="img-box" v-for="(item, index) in images" :key="index" ref="img">
                <img :src="item.img" alt="" @error="handleImageError">
            </div>
            <footer v-if="isLoad == false"
                        :style="{position: 'absolute', top: Math.max(...heightArray)+'px', color: 'red', left: '50%', transform: 'translateX(-50%)'}">
                没有图片加载了...
            </footer>
        </div>

        <!-- 留言板 -->
        <div class="message-board" v-if="currentView === 'messages'">
        <CommentsMessage ref="commentsMessage" :comments="comments" :userId=this.userID :othersId=this.curID :userAvatar=this.ownProfile.avatar @new-comment="handleNewComment" />
        </div>

      
    </div>
 </div>

    <!-- 时间轴容器 -->
    <div class="timeline-container">
      <div style="height: 10px;"></div>
      <div v-for="(location, index) in locations" :key="index" class="timeline-node" :class="{'active': index === 0}">
        <div class="node-dot"></div>
        <div class="node-info">
          <p>{{ location.address }}</p>
          <span>{{ location.timestamp }}</span>
        </div>
      </div>
    </div>

    <div class="classManageBox">
            <link href="https://cdn.bootcdn.net/ajax/libs/font-awesome/6.2.1/css/all.min.css" rel="stylesheet">
            <div class="share" :style="{bottom:addBottom+'px'}">
            <input type="checkbox" id="icon">
            <label for="icon">
                <i class="cticon icon-qiehuanbanji" style="--d:1" @click="changeClass"></i>
                <i class="cticon icon-jiarubanji " style="--d:2" @click="joinClass"></i>
                <i class="cticon icon-a-weibiaoti-1_huaban1fuben110" style="--d:3" @click="newClass"></i>

                <i class="cticon icon-teaching-class-import" style="--d:0"></i>
            </label>
            </div>
        </div>
          <!--加入班级-->
          <join-class-vue v-show="isJoining==1" @closeJoin="closeJoin"></join-class-vue>

<!--新建班级-->
<new-class-vue v-show="isCreating==1" @closeCreate="closeCreate"></new-class-vue>

<!--班级管理-->
<manage-class-vue v-show="isManaging==1" @closeManage="closeManage" @switched="classSwitched"></manage-class-vue>
    <foot-bar-vue></foot-bar-vue>
</template>

<script>


import topBar from '@/components/TopBar.vue';
import footBarVue from '@/components/FootBar.vue';

import CommentsMessage from '../components/comments/CommentsMessage.vue';

import SignatureModal from '../components/SignatureModal.vue';

//import Axios from 'axios'

import {  onMounted } from 'vue';

import { personalApi, settingApi, classmateApi } from '@/api/index';

import joinClassVue from '@/components/JoinClass.vue'
import newClassVue from '@/components/NewClass.vue'
import manageClassVue from '@/components/ManageClass.vue'

export default {
  inject: ['reload'],
    data() {
        return {
          addBottom: 50, // add按钮的bottom距离变量
      isJoining: 0,
            isCreating: 0,
            isManaging: 0,
            curID: -1,
            signature: 'To Remember Forever.', // Initial signature content
            isModalVisible: false, // Controls modal visibility


            isSidebarVisible: false, // 控制侧边栏显示状态
            editable: false, // 控制是否可编辑状态
            currentView: 'photos', // 默认显示照片墙

            userProfile: {
                avatar: '', // 默认头像路径
                // sign: 'This is a default signature.', // 默认签名
                name: 'Qiyuan Cheng', // 示例姓名，这里你可以设置为实陃用户姓名数据
                phone: '123-456-7890',  // 示例电话，这里你可以设置为实际用户电话数据
                email: '666@163.com',
                password: '77777',
                birthday: '2001-01-01',
                gender: 4, // 默认性别为未知
            },
            ownProfile: { // 账号所有者的profile
                avatar: '', // 默认头像路径
                // sign: 'This is a default signature.', // 默认签名
                name: 'Qiyuan Cheng', // 示例姓名，这里你可以设置为实陃用户姓名数据
                phone: '123-456-7890',  // 示例电话，这里你可以设置为实际用户电话数据
                email: '666@163.com',
                password: '77777',
                birthday: '2001-01-01',
                gender: 4, // 默认性别为未知  
            },
            images: [
                { img: require('/static/0.webp'), name: "111", PictureId: 1, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 10, GatheringId: null, Address: "Location 1", Description: "Description 1" },
                { img: require('/static/1.webp'), name: "222", PictureId: 2, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 20, GatheringId: null, Address: "Location 2", Description: "Description 2" },
                { img: require('/static/2.webp'), name: "333", PictureId: 3, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 30, GatheringId: null, Address: "Location 3", Description: "Description 3" },
                { img: require('/static/3.webp'), name: "444", PictureId: 4, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 40, GatheringId: null, Address: "Location 4", Description: "Description 4" },
                { img: require('/static/4.webp'), name: "555", PictureId: 5, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 50, GatheringId: null, Address: "Location 5", Description: "Description 5" },
                { img: require('/static/5.webp'), name: "666", PictureId: 6, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 60, GatheringId: null, Address: "Location 6", Description: "Description 6" },
                { img: require('/static/0.webp'), name: "111", PictureId: 7, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 70, GatheringId: null, Address: "Location 7", Description: "Description 7" },
                { img: require('/static/1.webp'), name: "222", PictureId: 8, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 80, GatheringId: null, Address: "Location 8", Description: "Description 8" },
                { img: require('/static/2.webp'), name: "333", PictureId: 9, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 90, GatheringId: null, Address: "Location 9", Description: "Description 9" },
                { img: require('/static/3.webp'), name: "444", PictureId: 10, ClassId: 1, ClassmateId: 1, Date: new Date(), Likes: 100, GatheringId: null, Address: "Location 10", Description: "Description 10" },
                { img: require('/static/4.webp'), name: "555", PictureId: 11, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 110, GatheringId: null, Address: "Location 11", Description: "Description 11" },
                { img: require('/static/5.webp'), name: "666", PictureId: 12, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 120, GatheringId: null, Address: "Location 12", Description: "Description 12" },
                { img: require('/static/0.webp'), name: "111", PictureId: 13, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 130, GatheringId: null, Address: "Location 13", Description: "Description 13" },
                { img: require('/static/1.webp'), name: "222", PictureId: 14, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 140, GatheringId: null, Address: "Location 14", Description: "Description 14" },
                { img: require('/static/2.webp'), name: "333", PictureId: 15, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 150, GatheringId: null, Address: "Location 15", Description: "Description 15" },
                { img: require('/static/3.webp'), name: "444", PictureId: 16, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 160, GatheringId: null, Address: "Location 16", Description: "Description 16" },
                { img: require('/static/4.webp'), name: "555", PictureId: 17, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 170, GatheringId: null, Address: "Location 17", Description: "Description 17" },
                { img: require('/static/5.webp'), name: "666", PictureId: 18, ClassId: 1, ClassmateId: 2, Date: new Date(), Likes: 180, GatheringId: null, Address: "Location 18", Description: "Description 18" }
            ],
            classmates: [
                
            ],
            imgWidth: 220, //图片的宽度
            heightArray: [],  //存储高度数组，用于判断最小高度的图片位置
            isLoad: true,  //是否继续加载图片
            surplusW: 0, //是否存在剩余宽度
            offsetP: 0,
            count: 0,


            locations: [
            { address: 'Seattle', timestamp: '2024-11-05 09:00' },
            { address: 'San Francisco', timestamp: '2024-11-03 08:15' },
            { address: 'Los Angeles', timestamp: '2024-11-02 14:30' },
            { address: 'New York', timestamp: '2024-11-01 10:00' },
            { address: 'Boston', timestamp: '2024-10-31 12:00' },
            { address: 'Chicago', timestamp: '2024-10-30 09:30' },
            { address: 'Houston', timestamp: '2024-10-29 08:00' },
            { address: 'Miami', timestamp: '2024-10-28 15:00' },
            ],
        };
    },
    
    computed: {
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
        CommentsMessage,
        SignatureModal,
        joinClassVue,
        newClassVue,
        manageClassVue,
    },
    methods: {
      classSwitched() {
        this.reload();
      },
    
        async editPersonalInfo() {
        try{
          const infoData = {
            classmate_id: this.userID,
            username: this.userProfile.name,
            sign: this.signature, // 修改签名
            birthdayString: this.userProfile.birthday,
            mail: this.userProfile.email,
            gender: this.userProfile.gender,
            headPicture: this.userProfile.avatar
          };
          const response = await personalApi.updatePersonalInfo(infoData);
          console.log('Personal info edit successful:', response);
          // Handle success response here
        }catch(error){
          console.error('Error editting personal info:', error);
          // Handle error here
        }
      },
        async handleImageError(event) {
            event.target.src = require('@/assets/images/headpoint.jpg');
            console.log("++++++++++图片加载失败，用流萤代替+++++++++++")
        },
        async returnToOwnPage() {
            this.curID = this.userID;
            this.initPage();
        },
        async handleClassmateClick(item) {
            this.curID = item.id;
            this.initPage();
        },
        async initPage() {
            await this.getPersonalInfo(this.curID);
            await this.getPersonalAdd(this.curID);
            await this.getClassmateList(this.classID);
            if (this.currentView === 'photos') {
              this.images = [];
              this.getPersonalPictures(this.curID);
                this.loadImgHeight();
                //得到页面的宽度
                const pageWidth_padding = this.$refs.box.clientWidth
                //页面的padding像素
                this.offsetP = this.$refs.box.style.paddingLeft.replace(/[^0-9]/ig, "")
                //获得页面的真实宽度（除去padding、margin、border...）
                const pageWidth = pageWidth_padding - (this.offsetP * 2)
                //计算出当前页面可展示多少列图片
                const column = Math.floor(pageWidth / this.imgWidth)
                //偏移像素值
                this.surplusW = pageWidth - (column * this.imgWidth)
                //初始化存储高度数组
                this.heightArray = []
                for (let i = 0; i < column; i++) {
                this.heightArray.push(0)
                }
                window.addEventListener('scroll', function () {
            //得到可滚动距离
            const scrollDistance = document.documentElement.scrollHeight - document.documentElement.clientHeight;
            //滚动到顶部的距离
            const scroll = document.documentElement.scrollTop;
            if (scrollDistance == scroll) {
            // 注释掉Axios请求
            // Axios({
            //     url: '/path/to/your/valid/endpoint.json',
            //     method: 'get'
            // }).then(res => {
            //     if (res.data.code == 200) {
            //     _this.count += 1
            //     if(_this.count == 4) {
            //         _this.isLoad = false
            //     }
            //     if(_this.isLoad) {
            //         const start = _this.images.length
            //         for (let item of res.data.data) {
            //         _this.images.push(item)
            //         }
            //         //滑到底部继续加载图片，this.$nextTick()异步加载，待资源虚拟DOM加载完毕
            //         _this.$nextTick(() => {
            //         _this.positionImg(start)
            //         })
            //     }
            //     }
            // })
            
            // 直接从this.images加载图片
            const start = this.images.length;
            this.$nextTick(() => {
                this.positionImg(start);
            });
            }
        })
               ;
                
            } else {
                this.getPersonalComment(this.curID);
            }


        },
        // 读取个人照片
        async getPersonalPictures(user_id) {
            try{
                const response = await personalApi.getPersonalPictures(user_id);
                console.log('user:', user_id, '; Personal pictures get successful:', response);
                // Handle success response here
                this.images = response.map(item => ({
                    img: item.image,
                    name: item.pictureId,
                    PictureId: item.pictureId,
                    ClassId: item.classId,
                    ClassmateId: item.classmateId,
                    Date: item.Date,
                    Likes: item.likes,
                    GatheringId: item.gatheringId,
                    Address: item.address,
                    Description: item.description
                }));
                console.log('Images:', this.images);
                }catch(error){
                console.error('Error getting personal pictures:', error);
                // Handle error here
            }
        },
        // //发布1、2级留言的方法分别在/components/comments下的两个组件内部
        // async postComment() {
        //     try{
        //     const commentData = {
        //         comment: '今天天气真好', 
        //         others_id: 2,  
        //         user_id: 3,   // user_id用户跑到others_id用户的个人空间里给others_id用户发了一条留言
        //     };
        //     const response = await personalApi.postComment(commentData);
        //     console.log('Comment post successful:', response);
        //     // Handle success response here
        //     }catch(error){
        //     console.error('Error posting comment:', error);
        //     // Handle error here
        //     }
        // },
        async handleNewComment(comment) {
            // 处理新的评论
            console.log('New comment:', comment);
            // 你可以在这里添加逻辑，比如将新评论添加到 comments 数组中
            this.getPersonalComment(this.curID);
            this.$nextTick(() => {
                // DOM更新完成后的操作
            });
        },
        //获取个人留言
        async getPersonalComment(user_id) {
            try{
            const response = await personalApi.getPersonalComments(user_id);
            console.log('Personal comment get successful:', response);
            // Handle success response here
            let commentList = response;
            console.log('CommentList:', commentList);
            let replyList = [];

            for (let i = 0; i < commentList.length; i++) {
                const replies = await this.getCommentDetail(commentList[i].comID);
                console.log('Replies:', replies);
                replyList = replyList.concat(replies);
            }

            this.$refs.commentsMessage.getComments(commentList, replyList);

            console.log('commentList', commentList);
            console.log('replyList', replyList);
            }catch(error){
            console.error('Error getting personal comment:', error);
            // Handle error here
            }
        },
        async getCommentDetail(com_id) {
            try{
            const response = await personalApi.getCommentDetails(com_id);
            console.log('ComID:', com_id, '; Comment detail get successful:', response);
            // Handle success response here
            return response;
            }catch(error){
            console.error('Error getting comment detail:', error);
            // Handle error here
            }
        },
        async getClassmateList(class_id) {
        try{
          const response = await classmateApi.getClassmateList(class_id);
          console.log('Classmate list get successful:', response);
          // Handle success response here
          this.classmates = response.map(item => ({ name: item.name, id: item.classmateId }));
        }catch(error){
          console.error('Error getting classmate list:', error);
          // Handle error here
        }
      },
        async getPersonalAdd(user_id) {
        try{
          const response = await personalApi.getPersonalAddress(user_id);
          console.log('Personal address get successful:', response);
          // Handle success response here
          const addStr = response.addressHistory; 
          console.log('addStr', addStr);
        let addArr = addStr && addStr.trim() ? addStr.split(" ") : [];
        addArr.reverse();
        console.log('splited', addArr);
        let locations = [];
        let currentDate = new Date();
        for (let i = 0; i < addArr.length; i++) {
            let timestamp = new Date(currentDate.getTime() - i * 24 * 60 * 60 * 1000).toISOString().slice(0, 19).replace('T', ' ');
            locations.push({ address: addArr[i], timestamp: timestamp });
        }
        this.locations = locations;
        }catch(error){
          console.error('Error getting personal address:', error);
          // Handle error here
        }
    },
        async getPersonalInfo(user_id) {
          try{
            const response = await personalApi.getPersonalInfo(user_id);
            console.log('Personal info get successful:', response);
            this.signature = response.sign;
            this.userProfile.name = response.name;
              this.userProfile.email = response.mail;
              this.userProfile.password = response.password;
              this.userProfile.birthday = response.birthday;
              this.userProfile.gender = response.gender;
              this.userProfile.phone = response.phoneNumber;
              this.userProfile.avatar = response.headPicture;
              console.log('Personal info:', this.userProfile);
              this.$nextTick(() => {
              // DOM更新完成后的操作
              });

            // Handle success response here
          }catch(error){
            console.error('Error getting personal info:', error);
            // Handle error here
          }
      },
        openSignatureModal() {
            this.isModalVisible = true;
        },
        closeModal() {
            this.isModalVisible = false;
        },
        async saveSignature(newSignature) {
            this.signature = newSignature;
            await this.editPersonalInfo();
            this.closeModal(); // Close the modal after saving

        },
        addComment() {
            if (this.newComment.trim() !== '') {
                this.comments.push(this.newComment);
                this.newComment = '';
            }
        },
        
        toggleSidebar() {
            this.isSidebarVisible = !this.isSidebarVisible;
            this.$nextTick(() => {
            // DOM更新完成后的操作
            });
        },
        showPhotoWall() {
            this.images = [];
            this.currentView = 'photos';
            
            // this.$nextTick(() => {  // DOM更新完成后的操作
            //     this.getPersonalPictures(this.curID);
            //     this.$nextTick(() => {  // 再次确保DOM更新
            //     this.init();
            //     this.positionImg(0);
            //     });
            // });
            this.initPage();
            },
        showMessageBoard() {
            this.currentView = 'messages';
            this.initPage();
        },
        loadImgHeight() {
            let count = 0;
            let allImagesLoaded = this.images.every(item => {
                let image = new Image();
                image.src = item.img;
                image.onload = image.onerror = () => {
                count++;
                if (count === this.images.length) {
                    this.$nextTick(() => {
                    this.init();
                    this.positionImg(0);
                    });
                }
                };
                return image.complete;
            });
            if (!allImagesLoaded) {
                console.error('Some images are not loaded');
            }
        },
      init() {
        //得到页面的宽度
        const pageWidth_padding = this.$refs.box.clientWidth
                //页面的padding像素
                this.offsetP = this.$refs.box.style.paddingLeft.replace(/[^0-9]/ig, "")
                //获得页面的真实宽度（除去padding、margin、border...）
                const pageWidth = pageWidth_padding - (this.offsetP * 2)
                //计算出当前页面可展示多少列图片
                const column = Math.floor(pageWidth / this.imgWidth)
                //偏移像素值
                this.surplusW = pageWidth - (column * this.imgWidth)
                //初始化存储高度数组
                for (let i = 0; i < column; i++) {
                this.heightArray.push(0)
                }
      },
      positionImg(start) {
        //获得img标签的父容器的DOM
        let parentDom = this.$refs.img
        for (let i = start; i < this.images.length; i++) {
          //获得最小高度
          const minHeight = Math.min(...this.heightArray)
          //获得最小高度索引
          const index = this.heightArray.indexOf(minHeight)
          //获得当前图片的高度
          const currHeight = parentDom[i].clientHeight
          //定位
          parentDom[i].style.transform = '50px'
          parentDom[i].style.position = 'absolute'
          parentDom[i].style.top = minHeight + 'px'
          parentDom[i].style.left = this.imgWidth * index + + ((Math.floor((this.surplusW / 2)) + 30)) +  'px'
          this.heightArray[index] += currHeight
        }
        //对父容器赋值当前heightArray数组的最大高度
        this.$refs.box.style.height = Math.max(...this.heightArray) + 50 + 'px'
      },

        async fetchUserData(){
            try{
                console.log("正在查询");
                const response = await settingApi.getUserInfo(this.userID);
                console.log('查询得到信息:', response);
                const userData = response;
                this.ownProfile.name = userData.name;
                this.ownProfile.email = userData.mail;
                this.ownProfile.password = userData.password;
                this.ownProfile.avatar = userData.headPicture;
                if (userData && userData.birthday) {
                    this.ownProfile.birthday = userData.birthday.substring(0, 10);
                } else {
                    this.ownProfile.birthday=null;
                }
                this.ownProfile.gender = userData.gender;
                this.ownProfile.phone=+userData.phoneNumber;
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
        handleAvatarChange(event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.userProfile.avatar = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        },
        selectLocation() {
            if (this.editable) {
                // 处理位置选择逻辑
                console.log('位置选择');
            }
        },
        changeClass() {
            this.isManaging = 1;
        },
        closeManage() {
            this.isManaging = 0;
        },
        joinClass() {
            this.isJoining = 1;
        },
        closeJoin() {
            this.isJoining = 0;
        },
        newClass() {
            this.isCreating = 1;
        },
        closeCreate() {
            this.isCreating = 0;
        }
        
    },

    mounted() {
      const _this = this
     
      
      //监听滚动条滚动，实现懒加载图片
        window.addEventListener('scroll', function () {
            //得到可滚动距离
            const scrollDistance = document.documentElement.scrollHeight - document.documentElement.clientHeight;
            //滚动到顶部的距离
            const scroll = document.documentElement.scrollTop;
            if (scrollDistance == scroll) {
            // 注释掉Axios请求
            // Axios({
            //     url: '/path/to/your/valid/endpoint.json',
            //     method: 'get'
            // }).then(res => {
            //     if (res.data.code == 200) {
            //     _this.count += 1
            //     if(_this.count == 4) {
            //         _this.isLoad = false
            //     }
            //     if(_this.isLoad) {
            //         const start = _this.images.length
            //         for (let item of res.data.data) {
            //         _this.images.push(item)
            //         }
            //         //滑到底部继续加载图片，this.$nextTick()异步加载，待资源虚拟DOM加载完毕
            //         _this.$nextTick(() => {
            //         _this.positionImg(start)
            //         })
            //     }
            //     }
            // })
            
            // 直接从this.images加载图片
            const start = _this.images.length;
            _this.$nextTick(() => {
                _this.positionImg(start);
            });
            }
        })
    
    
      
    },
    unmounted() {
        window.removeEventListener('scroll', function () {
            console.log('滚动事件已移除');
        });
    },
    created() {

        this.curID = this.userID;
        this.initPage();
        this.fetchUserData();


     /* Axios({
        url: '/waterFall.json',
        method: 'get'
      }).then(res => {
        if (res.data.code == 200) {
          this.images = res.data.data ? res.data.data : []
          this.loadImgHeight()
        }
      })*/
    },
    setup() {

        onMounted(() => {
            
        });
    }
};
</script>

<style lang="less" scoped>
.ps-index { 
    width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  /* height: 100vh; */
  position: relative;
  overflow: hidden;
   min-height: 80vh; 
}

.bg-video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  z-index: -1;
}

.user-profile {
  display: flex;
  flex-direction: column;
  align-items: center;

  position: relative;
  padding-top: 30vh;
margin: 20px auto 0; /* 调整上边距，确保不会被顶出视口 */

  width: 100%;
  max-width: 800px; /* 限制最大宽度 */
  padding: 20px;
  background: rgba(96, 144, 222, 0.2); /* 透明度调整为80% */
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  z-index: 0;
}

.user-avatar {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  margin-bottom: 20px;
  border: 2px solid #fff; /* 增加白色边框 */
}

.signature {
  text-align: center; /* 签名居中 */
  font-size: 24px; /* 较大的字体 */
  font-weight: bold; /* 加粗 */
  color: #333; /* 深色文字 */
  margin-bottom: 15px; /* 与下方信息的间距 */
  text-transform: uppercase; /* 大写 */
  letter-spacing: 2px; /* 字间距 */
}
.signature-section{
    display: flex;
  text-align: center; /* 签名居中 */
  font-size: 24px; /* 较大的字体 */
  font-weight: bold; /* 加粗 */
  color: #333; /* 深色文字 */
  margin-bottom: 15px; /* 与下方信息的间距 */
  text-transform: uppercase; /* 大写 */
  letter-spacing: 2px; /* 字间距 */
}
.signature-modal {
  max-width: 270px;
  padding: 20px;
  background: linear-gradient(135deg, rgba(96, 144, 222, 0.2), rgba(250, 208, 196, 0.2));
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  z-index: 1002;
  animation: slideIn 0.5s ease-in-out;

  @keyframes slideIn {
  from {
    opacity: 0;
    transform: translate(-50%, -60%);
  }
  to {
    opacity: 1;
    transform: translate(-50%, -50%);
  }
}
}

.signature-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.signature-modal-header h2 {
  color: #fff;
  font-size: 24px;
  margin: 0;
}

.signature-modal-header button {
  background: none;
  border: none;
  color: #fff;
  font-size: 20px;
  cursor: pointer;
}

.signature-modal-body {
  margin-bottom: 20px;
}

.signature-modal-body textarea {
  width: 100%;
  height: 100px;
  padding: 10px;
  border: 2px solid #6e8efb;
  border-radius: 10px;
  resize: none;
  font-size: 16px;
  background: linear-gradient(135deg, rgba(96, 144, 222, 0.2), rgba(250, 208, 196, 0.2));
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  color: #333;
  transition: border-color 0.3s, box-shadow 0.3s;
}

.signature-modal-body textarea:focus {
  border-color: #3a8efb;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  outline: none;
}

.signature-modal-footer {
  display: flex;
  justify-content: flex-end;
}

.signature-modal-footer button {
  background: #fff;
  color: #6e8efb;
  border: none;
  padding: 10px 20px;
  border-radius: 10px;
  cursor: pointer;
  transition: background 0.3s;
}

.signature-modal-footer button:hover {
  background: #f1f1f1;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translate(-50%, -60%);
  }
  to {
    opacity: 1;
    transform: translate(-50%, -50%);
  }
}

.user-info {
  display: flex;
  flex-wrap: wrap;
  gap: 20px; /* 信息之间的间距 */
}

.form-item {
  display: flex;
  align-items: center;
  gap: 10px; /* 标签和内容之间的间距 */
  width: calc(50% - 10px); /* 两两并排显示 */
}

.pslabel {
  font-size: 18px; /* 增大字体大小 */
  font-weight: bold; /* 加粗 */
  color: #333;
}

.psspan {
  font-size: 18px; /* 增大字体大小 */
  font-weight: bold; /* 加粗 */
  color: #666;
}

.photo-wall {
  width: 100%;
  margin-top: 20px;
}

.memory-section {
  margin-bottom: 20px;
}

.memory-items {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.memory-item {
  width: calc(33.33% - 10px);
  display: flex;
  flex-direction: column;
  align-items: center;
}

.memory-img {
  width: 100%;
  height: auto;
  border-radius: 10px;
}

.memory-name {
  font-size: 16px;
  font-weight: bold;
  margin-top: 5px;
}

.memory-des {
  font-size: 14px;
  color: #666;
  text-align: center;
}
.waterFall-box {
    position: relative;
    text-align: center;
    overflow-y: hidden;
  }

  .waterFall-box .img-box {
    width: 210px;
    display: block;
    float: left;
  }

  .waterFall-box .img-box img {
    width: 100%;
    animation: imgBox .5s ease-in-out;
  }

  .waterFall-box .img-box img:hover {
    transform: translateY(-3px);
    transition: transform .5s ease-in-out;
    box-shadow: 0 20px 20px 2px #737373;
  }

  @keyframes imgBox {
    0%{
      opacity: 0;
      transform: translateY(-100px);
    }
    100% {
      opacity: 1;
      transform: translateX(0);
    }
  }

/* 容器样式 */
.container-with-menu {
    width: 100%; /* 容器宽度自适应填满整个页面 */
    max-width: 1200px; /* 可以设置一个最大宽度，根据设计需求 */
    margin: 0 auto; /* 水平居中 */
    padding: 20px; /* 根据需要添加一些内边距 */
    box-sizing: border-box; /* 确保宽度包括内边距和边框 */
  }

  /* 菜单栏样式 */
.menu-bar {
    display: flex; /* 使用flex布局 */
    justify-content: center; /* 菜单项居中 */
    gap: 10px; /* 菜单项之间的间距 */
    margin-bottom: 20px; /* 与照片墙之间的间距 */
    background: rgba(96, 144, 222, 0.2); /* 透明度调整为80% */
    border-radius: 10px;
    width: 100%;
  max-width: 850px; /* 限制最大宽度 */
    
  }

  .menu-bar button {
  padding: 10px 15px; /* 调整按钮内边距，使其更小 */
margin: 20px 60px; /* 竖直外边距20px，水平外边距60px */
  border: none; /* 无边框 */
  background: rgba(39, 101, 202, 0.7); 
  color: white; /* 文字颜色 */
  border-radius: 4px; /* 圆角边框，稍微小一点 */
  cursor: pointer; /* 鼠标悬停时显示指针 */
  transition: background-color 0.3s; /* 背景色渐变效果 */
}

  .menu-bar button:hover {
    background-color: #0056b3; /* 鼠标悬停时的背景颜色 */
  }


.pssidebar {
    position: fixed; /* 固定定位 */
  left: 0px; /* 紧贴页面左侧 */
  top: 6vh; /* 从页面顶部开始 */
  width: 250px; /* 设置侧边栏的宽度 */
  height: 100%; /* 侧边栏高度占满整个视口高度 */
  background-color: #343a40; /* 深灰色背景 */
  /* border-radius: 10px; 圆角 */
  box-shadow: 5px 0 15px rgba(0,0,0,0.5); /* 增强阴影效果 */
  padding-top: 20px; /* 顶部内边距 */
  overflow-y: auto; /* 允许垂直滚动 */
  z-index: 1000; /* 确保侧边栏在其他元素之上 */
  transition: transform 2s ease; /* 平滑过渡效果 */
  /* transform: translateX(-50%);  */
  /*初始状态在屏幕外 */
  /* 按钮在sidebar中居中 */
  display: flex; /* 设置为flex容器 */
  flex-direction: column; /* 子元素垂直排列 */
  align-items: center; /* 子元素在交叉轴上居中 */
}

/* .sidebar.visible {
  transform: translateX(20%); /* 可见状态在屏幕内 
} */

/* 侧边栏标题样式 */
.pssidebar-title {

  padding: 10px 20px; /* 内边距 */
  font-size: 18px; /* 字体大小 */
  font-weight: bold; /* 加粗 */
  color: #fff; /* 文字颜色 */
  background-color: transparent; /* 背景颜色 */
  text-align: center; /* 文本居中 */
  margin-bottom: 10px; /* 与按钮列表的间距 */
}

.pssidebar-button {
  width: 92%; /* 按钮宽度占满侧边栏 */
  padding: 15px; /* 按钮的内边距 */
  margin-bottom: 8px; /* 按钮之间的间距 */
  border:#007bff;
  
  background-color: #495057; /* 按钮背景颜色 */
  color: white; /* 白色文字 */
  cursor: pointer; /* 鼠标悬停时显示指针 */
  transition: background-color 0.3s; /* 背景色渐变效果 */
  border-radius: 5px; /* 按钮圆角 */

}


.pssidebar-button:hover {
  background-color: #6c757d; /* 鼠标悬停时的背景颜色 */
}


.toggle-button {
  position: fixed; /* 固定定位 */
  top: 60px; /* 距离顶部20px */
  left: 10px; /* 距离左侧20px */
  width: 50px; /* 按钮宽度 */
  height: 50px; /* 按钮高度 */
  background-color: #007bff; /* 蓝色背景 */
  color: white; /* 白色文字 */
  border: none; /* 无边框 */
  border-radius: 50%; /* 圆形 */
  box-sizing: border-box;
  cursor: pointer; /* 鼠标悬停时显示指针 */
  font-size: 30px; /* 字体大小 */
  text-align: center; /* 水平居中文字 */
  box-shadow: 0 4px 8px rgba(0,0,0,0.2); /* 添加阴影 */
  transition: background-color 0.3s; /* 背景色渐变效果 */
  z-index: 1001; /* 确保按钮在其他元素之上 */
}

.toggle-button:hover {
  background-color: #0056b3; /* 鼠标悬停时的背景颜色 */
}

.message-board {
  max-width: 1000px;
  margin: 0 auto;
  padding: 20px;
  background: rgba(63, 128, 233, 0.4); /* 透明度调整为80% */
  border-radius: 8px;
  box-shadow: 0 4px 6px 10px rgba(0,0,0,0,0.1);
}

.messages {
  margin-bottom: 20px;
}

.message {
  padding: 10px;
  margin-bottom: 10px;
  border-bottom: 1px solid #ddd;
}

form {
  display: flex;
}

input {
  flex: 1;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.message-board {
    padding: 20px;
}

input {
    margin-right: 10px;
}


/* 时间轴容器样式 */
.timeline-container {
  position: fixed; /* 固定定位 */
  right: 10px; /* 右边距 */
  top: 10vh; /* 距离页面顶部 */
  width: 220px; /* 宽度 */
  background: rgba(255, 255, 255, 0.8); /* 背景色，半透明 */
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
}

/* 时间轴节点样式 */
.timeline-node {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  position: relative;
  padding-left: 30px; /* 留出空间放置圆点 */
}

.timeline-node .node-dot {
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background-color: grey; /* 默认灰色 */
}

.timeline-node.active .node-dot {
  background-color: green; /* 当前节点绿色 */
}

.timeline-node .node-info {
  font-size: 14px;
  color: #333;
}

.timeline-node .node-info p {
  margin: 0;
  font-weight: bold;
}

.timeline-node .node-info span {
  font-size: 12px;
  color: #888;
}
.modify-button {
  background: none;
  border: none;
  cursor: pointer;
  margin-left: 10px;
}

.modify-button img {
  width: 16px;
  height: 16px;
}

.return-button {
    position: fixed; /* 固定定位 */
    top: 60px; /* 距离顶部60px */
    right: 10px; /* 距离右侧10px */
    width: 150px; /* 按钮宽度 */
    height: 50px; /* 按钮高度 */
    background-color: #28a745; /* 绿色背景 */
    color: white; /* 白色文字 */
    border: none; /* 无边框 */
    border-radius: 25px; /* 圆角边框 */
    cursor: pointer; /* 鼠标悬停时显示指针 */
    font-size: 18px; /* 字体大小 */
    line-height: 50px; /* 垂直居中文字 */
    text-align: center; /* 水平居中文字 */
    box-shadow: 0 4px 8px rgba(0,0,0,0.2); /* 添加阴影 */
    transition: background-color 0.3s; /* 背景色渐变效果 */
    z-index: 1001; /* 确保按钮在其他元素之上 */
}

.return-button:hover {
    background-color: #218838; /* 鼠标悬停时的背景颜色 */
}

.classManageBox {
        .share {
            position: fixed;
            left: 30px;
            transition: @tr;
        }

        .share input {
            display: none;
        }

        .share label {
            display: flex;
            justify-content: center;
            align-items: center;
            width: 56px;
            height: 56px;
            border-radius: 50%;
            background: white;
            box-shadow: 0px 4px 8px 0px rgba(0,0,0,0.08);
        }

        .share input:checked~label i {
            background: rgba(53, 152, 228, 0.141);
        }

        .share label i {
            position: absolute;
            font-size: 30px;
            width: 56px;
            height: 56px;
            background: white;
            border-radius: 50%;
            display: flex;
            justify-content: center;
            align-items: center;
            transition: 0.3s linear;
            color: @gray-1;
        }

        .share label i::after {
            content: "";
            position: absolute;
            width: 56px;
            height: 56px;
            border-radius: 50%;
            transition: 0.4s;
        }

        .share label i:hover::after {
            background: rgba(0, 136, 255, 0.353);
        }

        .share input:checked~label i:nth-child(1) {
            transform: translate(0, -80px);
        }

        .share input:checked~label i:nth-child(2) {
            transform: translate(70px, -40px);
        }

        .share input:checked~label i:nth-child(3) {
            transform: translate(70px, 40px);
        }

        .share label i:nth-child(4) {
            transition: 0.8s;
        }

        .share input:checked~label i:nth-child(4) {
            transform: rotate(360deg);
            transition: 0.8s;
        }
    }

</style>