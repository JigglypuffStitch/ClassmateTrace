<template>
    <div class="card-detail">
        <div class="top-bt">
            <p class="revoke" @click="deleteCard" :class="{notmine: !isMine}">删除</p>
            <p class="report" v-show="id==1" @click="tagMyself">{{ isTagged?tagInfo[0]:tagInfo[1] }}</p>
        </div>
        <word-card-vue :word="card"></word-card-vue> <!--内容变化之后再弄-->
        <div class="form">
            <textarea class="ccomment" placeholder="请输入..." v-model="ccontent"></textarea> <!--拿到内容ccontent-->
            <div class="bt">
                <input type="text" class ="name" placeholder="签名" v-model="name"/>
                <ct-buttons-vue :class="{notallowed: !isDis}" @click="submit">确定</ct-buttons-vue> <!--加点击事件-->
            </div>
        </div>
        <p class="ctitle">评论{{ cards.comcount[0].count }}</p> <!--评论数量可以拿到，现在是cards-->
        <div class="comments">
            <div class="comments-li" v-for="(e,index) in comments" :key="index"> <!--v-for 后才能拿到 e-->
                <div class="user-head" :style="{backgroundImage: portrait[e.imgurl]}"></div>
                <div class="comm-m">
                    <div class="m-top">
                        <p class="name">{{ e.name }}</p>
                        <p class="time">{{ dataOne(e.moment) }}</p>
                    </div>
                    <div class="mm">{{ e.comment }}</div>
                </div>
            </div>
            <p class="more" @click="getComment" v-show="page>0">加载更多</p> <!--0让之消失-->
        </div>
    </div>
</template>

<script>
import wordCardVue from './WordCard.vue'
import ctButtonsVue from './CTButtons.vue'
// import {comment} from '../../mock/index'
import {portrait} from '@/utils/data'
import { dataOne } from '@/utils/methods'
import { insertCommentApi, findCommentPageApi, deleteCardApi, commentApi, albumApi, classApi } from '@/api/index'
export default {
    data() {
        return {
            comments: [],
            portrait, 
            dataOne,
            ccontent: '', // 输入内容
            name: '匿名', // 用户名
            user: this.$store.state.user, 
            page: 1,
            pagesize: 3,
            tagInfo: ['取消圈出', '圈出自己'],
            istagged: false,
        }
    },
    props: {
        card: {  // 卡片详情信息
            default: {},
        },
        id: {
            default: 0,
        }
    },
    computed: {
        isDis() {
            if (this.ccontent && this.name) {
                return true;
            }
            else {
                return false;
            }
        },
        cards() {  // 多加一层
            return this.card; 
        },
        isMine() {
            let isMonitor = this.isMonitorOrNo()
            if (this.user.id == this.cards.userId || isMonitor) {
                return true;
            }
            else {
                return false;
            }
        },
        isTagged() {
            return this.istagged;
        }
    },
    components: {
        wordCardVue,
        ctButtonsVue,
    },
    methods: {
        async isMonitorOrNo() {
            let params = {
                userId: this.user.id,
                classId: this.$store.state.classId,
            }
            const response = await classApi.isMonitor(params)
            console.log(response)
            return response.result
        },
        // 提交评论
        submit() {
            if (this.isDis) {  // 有内容
                // 如果有用户头像，则用头像，不然就用随机头像
                let img = Math.floor(Math.random()*14); // 生成14内的随机数
                let data = {
                    wallId: this.cards.id,
                    userId: this.user.id,
                    imgurl: img,
                    moment: new Date(),
                    name: this.name,
                    comment: this.ccontent,
                };
                console.log(data)
                insertCommentApi(data).then(() => {
                    this.comments.unshift(data);
                    this.cards.comcount[0].count++;  // 这个不能直接改变props！要模仿word，要弄一层在computed中
                    // 清空评论框
                    this.ccontent = '';
                })
                this.$message({ type: "success", message: "评论成功" });
            }
        },
        // 获取评论 - 也要分页
        getComment() {
            // console.log(this.card)
            if (this.page > 0) {  // 取完了page会置0
                let data = {
                    id: this.card.id,
                    page: this.page,
                    pagesize: this.pagesize,
                }
                findCommentPageApi(data).then((res) => {
                    this.comments = this.comments.concat(res.message)
                    // 设置下一次的page
                    if (res.message.length == this.pagesize) {  // 取满
                        this.page++;
                    }
                    else {  // 取不满了
                        this.page = 0;
                    }
                    // console.log(data);
                })
            }
        },
        // 删除照片
        deleteCard() {
            // 拥有者才可以删除
            if (this.isMine) {
                let data = {
                    id: this.cards.id,
                    imgurl: this.cards.imgurl
                }
                // console.log(data)
                deleteCardApi(data).then(() => {
                    this.$emit("cardDeleted", data.id);
                    this.$message({ type: "success", message: "删除成功！" });
                    if (this.id == 0) {
                        this.deleteMessage(data.id);
                    }
                    else if (this.id == 1) {
                        this.deletePhoto(this.user.id, data.id);
                    }
                })
            }
            else {
                if (this.cards.type == 0) {
                    this.$message({ type: "error", message: "这不是你的留言，无法删除。" });
                }
                else {
                    this.$message({ type: "error", message: "这不是你的照片，无法删除。" });
                }

            }
        },
        // 获取圈出状态
        async ifTagged() {
            let params = {
                userId: this.user.id,
                photoId: this.cards.id,
            }
            const response = await albumApi.isTagged(params);
            console.log(response)
            this.istagged=response.result;
            console.log(this.istagged)
        },
        // 圈出自己
        async tagMyself() {
            if (this.istagged) {  // istagged为true
                this.untagMyself()
            }
            else {
                let data = {
                    classmateId: this.user.id,
                    pictureId: this.cards.id,
                }
                const response = await albumApi.markSelf(data);
                console.log(response)
                // api调用
                this.$message({ type: "success", message: "你已圈出自己！" });
                this.ifTagged();
            }
        },
        // 取消圈出自己
        async untagMyself() {
            let params = {
                classmate_id: this.user.id,
                picture_id: this.cards.id,
            }
            const response = await albumApi.unmarkSelf(params);
            console.log(response)
            // api调用
            this.$message({ type: "success", message: "你已取消圈出自己！" });
            this.ifTagged();
        },
        async deleteMessage(e) {
            const response = await commentApi.deleteComment(e);
            console.log(response)
        },
        async deletePhoto(x, y) {
            let params = {
                classmate_id: x,
                picture_id: y,
            }
            const response = await albumApi.deletePicture(params);
            console.log(response);
        }
    },
    mounted() {  // 放在外面！！小心！
        this.getComment(); // 第一次需要调用的
        this.ifTagged()
    },
    watch: {
        card(){
            // 卡片发生变化
            this.page = 1;
            this.comments = [];
            this.getComment();
        }
    }
}
</script>

<style lang="less" scoped>
.card-detail {
    position: relative;
    padding: 0 @padding-20;
    .top-bt {
        // border: 1px solid red; // 辅助线
        position: fixed;
        top: 0;
        left: 0;
        padding: @padding-20;
        display: flex;
        .revoke {
            color: @primary-color;
            cursor: pointer;
            padding-right: 30px;
            font-size: @size-16;
        }
        .report {
            font-size: @size-16;
            color: @warning-color;
            cursor: pointer;
        }
        .notmine {
            color: @gray-2;
            cursor: not-allowed;
        }
    }
    
    .form {
        .ccomment {
            background: none;
            height: 56px;
            border: 1px solid rgba(148,148,148,1);
            resize: none;  // 不可变化大小
            padding: @padding-8;
            box-sizing: border-box;
            width: 100%;
            margin-top: @padding-12;
        }
        .bt {
            display: flex;
            padding-top: 6px;
            justify-content: space-between;
        }
        .name {
            width: 200px;
            box-sizing: border-box;
            padding: 7px;
            background: none;
            border: 1px solid rgba(148,148,148,1);
            line-height: 20px;
        }
    }
    .ctitle {
        font-weight: 600;
        padding-top: 30px;
        padding-bottom: 20px;
    }
    .more {
        font-family: wordF;
        text-align: center;
        cursor: pointer;
        padding: 10px;
    }
    .comments-li {
        display: flex;
        padding-bottom: 30px; // 整体底部间距
        .user-head {
            flex: none; // 完整显示
            width: 28px;
            height: 28px;
            border-radius: 20px;
            // background-image: linear-gradient(180deg, #FFA9D9 0%, #E83D3D 100%); // 先静态
            overflow: hidden; // 放图片
        }
        .comm-m {
            padding-left: @padding-8;
        }
        .m-top {
            display: flex;
            align-items: center;
            .name {
                font-weight: 600;
            }
            .time {
                font-size: @size-12;
                padding-left: @padding-4;
                color: @gray-2;
            }
        }
        .mm {
            padding-top: @padding-4;
        }
    }
}

</style>