<template>
    <div class="msg-all-contain">
      <div class="msg-board-title">留言板</div>
      <div class="msg-board">
        <div class="msg-board-contain">
          <div class="msg-head">
            <img v-if="userAvatar" :src="userAvatar" alt="" />
            <img v-else :src="require('@/assets/images/headpoint.jpg')" alt="" />
            <textarea
              type="textarea"
              :class="inputStatusClass"
              placeholder="请输入内容..."
              ref="input"
              v-model="newComment"
              cols="60"
              rows="5"
            >
            </textarea>
            <button @click="submit">发表</button>
          </div>
          <div class="msg-content">
            <comments-child
              :comments="comments"
              :count="layerCount"
              :userId="userId" 
              @new-reply="handleNewReply"
            ></comments-child>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script>

import { personalApi } from '@/api/index';

  import CommentsChild from "@/components/comments/CommentsChild";
  import dayjs from "dayjs";
import { nextTick } from 'vue';
  export default {
    name: "commentsMessage",
    props: {
    userId: Number,
    othersId: Number,
    userAvatar: {
      type: String,
      default: "",
    }
  },
    data() {
      return {
        // 评论列表
        comments: [],
        // 新评论
        newComment: "",
        // 用户头像
        // 非空判断
        hasNoConent: false,
        // 输入栏状态
        inputStatusClass: "",
        // 计数
        layerCount: 0,
      };
    },
    created() {},
    mounted() {
      // this.initData();

    },
    components: {
      "comments-child": CommentsChild,
    },
    methods: {
      async getUserAvatarAndName(user_id) {
        try {
          const response = await personalApi.getPersonalInfo(user_id);
          await nextTick();
          console.log('Personal info get successful:', response);
          return {
              avatar: response.headPicture,
              userName: response.name 
          }
        } catch (error) {
          console.error('Error getting user avatar:', error);
          return null; // Return null or a default value in case of error
        }
      },
      // 处理新回复
      handleNewReply(newReply) {
        this.$emit("new-comment", newReply);
      },
      // 提交
      async submit() {
        var self = this;
        // 提交信息非空验证
        if (!self.newComment || self.newComment === "") {
          self.hasNoConent = true;
          self.inputStatusClass = "no-content-warn";
          return;
        }
        self.inputStatusClass = "";
  
        await self.postComment();
        
        self.$emit('new-comment', this.newComment);
        
        // 清空输入框
        self.newComment = "";
        // 添加到评论列表

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
      initData() {
        var self = this;
        self.makeComments();
      },
      
      async getComments(commentList, replyList) {
        var self = this;
        // 首先处理评论
        self.comments = await Promise.all(commentList.map(comment => (
          Promise.all([
            this.getUserAvatarAndName(comment.commentator), // 等待头像加载
          ]).then(([senderAvatarAndName]) => { // 通过某种神奇的异步逻辑解决二级评论和头像的冲突
            const { avatar, userName } = senderAvatarAndName; // 分解这个返回值
            return {
              parentId: null,
              text: comment.comment,
              senderId: comment.commentator,
              receiverId: comment.userID,
              postDate: dayjs().format("YYYY-MM-DD HH:mm"),
              senderAvatar: avatar, // 使用解构后的 avatar
              children: [],
              senderName: userName, // 使用解构后的 userName
              id: comment.comID,
              likes: 0,
              receiverName: "", // 修改点：这里本应该是this.userId对应的名字，但是要异步查询这个人到底叫什么的话铁定会出错。所以这里就不填了
              receiverAvatar: "",
            };
          }))));

        // 然后处理回复
        self.comments.forEach(async comment => {
          comment.children = await Promise.all(replyList
            .filter(reply => reply.reID === comment.id)
            .map(reply => (
              Promise.all([
                this.getUserAvatarAndName(reply.commentator), // 等待头像加载
              ]).then(([senderAvatarAndName]) => {
                const { avatar, userName } = senderAvatarAndName;
                return {
                  parentId: reply.reID,
                  text: reply.comment,
                  senderId: reply.commentator,
                  receiverId: reply.comID,
                  postDate: dayjs().format("YYYY-MM-DD HH:mm"),
                  senderAvatar: avatar, // 使用解构后的 avatar
                  children: [],
                  senderName: userName, // 使用解构后的 userName
                  id: reply.comID,
                  likes: 0,
                  receiverName: comment.senderName, // 修改点：父级评论的用户名字，可以直接引用   
                  receiverAvatar: "",
                };
              })
                  ))
          );
        });

        await nextTick(); // 确保 DOM 更新
      },
      makeComments() {
        var self = this;
        // 这是自己伪造的数据
        // 要根据接口要求进行修改
        self.comments = [
          {
            children: [
              {
                parentId: "55824b1c",
                text: "回复信息",
                senderId: "25d85a0f",
                receiverId: "25d85a0f",
                postDate: "2022-08-05 12:10",
                senderAvatar: "",
                children: [],
                senderName: "Wendy",
                id: "f0e3a81b",
                likes: 0,
                receiverName: "Irene",
                receiverAvatar: "",
              },
            ],
            id: "55824b1c",
            postDate: "2022-08-05 8:09",
            senderName: "Wendy",
            senderAvatar: "",
            receiverName: null,
            receiverAvatar: null,
            parentId: null,
            text: "测试信息",
            senderId: "25d85a0f",
            receiverId: null,
            likes: 0,
          },
        ];
      },
      async postComment() {
        try{
          const commentData = {
            comment: this.newComment,
            others_id: this.othersId,
            user_id:  this.userId, // user_id用户跑到others_id用户的个人空间里给others_id用户发了一条留言
          };
          const response = await personalApi.postComment(commentData);
          console.log('Comment post successful:', response);
          // Handle success response here
        }catch(error){
          console.error('Error posting comment:', error);
          // Handle error here
        }
      },
      
    },
  };
  </script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style scoped>
  /* 评论头像 */
  .msg-head img {
    width: 55px;
    height: 55px;
    border-radius: 50%;
    position: absolute;
    top: 22px;
    left: 13px;
  }
  .msg-all-contain {
    width: 100%;
    height: 100%;
    overflow-y: auto;
  }
  
  .msg-board-contain {
    letter-spacing: 1px;
    padding: 0 10px;
  }
  
  /* 信息栏标题 */
  .msg-board-title {
    width: auto;
    text-align: center;
    font-size: 28px;
    font-weight: 300;
    margin: 0 0 1.8rem 0;
    min-height: 20px;
    color: #000 !important;
    font-family: "Lato", Verdana, sans-serif !important;
  }
  .msg-head {
    background-color: rgb(248, 248, 248);
    position: relative;
    height: 130px;
    border-radius: 5px;
  }
  
  /* 评论输入 */
  .msg-head textarea {
    position: absolute;
    top: 13px;
    left: 85px;
    max-height: 60px;
    border-radius: 5px;
    outline: none;
    width: calc(100% - 300px);
    font-size: 16px;
    padding: 20px;
    border: 2px solid #f8f8f8;
    resize: none;
  }
  /* 发布评论按钮 */
  .msg-head button {
    position: absolute;
    top: 13px;
    right: 35px;
    width: 100px;
    height: 100px;
    border: 0;
    border-radius: 5px;
    font-size: 18px;
    font-weight: 500;
    color: #fff !important;
    background-color: #00a1d6;
    transition: 0.1s;
    cursor: pointer;
    letter-spacing: 2px;
  }
  /* 鼠标经过字体加粗 */
  .msg-head button:hover {
    /*font-weight: 600;*/
  }
  
  .msg-content {
    overflow-y: auto;
  }
  
  /* 评论内容区域 */
  .msg-content .child-comment {
    display: flex;
    position: relative;
    padding: 18px 10px 18px 10px;
  }
  
  @media (max-width: 900px) {
    .msg-head img {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      position: absolute;
      top: 22px;
      left: 13px;
    }
    .msg-head textarea {
      position: absolute;
      top: 13px;
      left: 70px;
      height: 55px;
      border-radius: 5px;
      outline: none;
      width: calc(100% - 200px);
      font-size: 15px;
      padding: 10px;
      border: 2px solid #f8f8f8;
      resize: none;
    }
    .msg-head button {
      position: absolute;
      top: 13px;
      right: 16px;
      width: 80px;
      height: 77px;
      border: 0;
      border-radius: 5px;
      font-size: 14px;
      font-weight: 500;
      color: #fff !important;
      background-color: #00a1d6;
      transition: 0.1s;
      cursor: pointer;
      letter-spacing: 2px;
    }
  }
  </style>