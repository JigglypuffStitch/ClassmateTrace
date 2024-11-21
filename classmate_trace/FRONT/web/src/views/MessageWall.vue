<template>
    <div class="wall-message">
        <!--依据id显示title和slogan-->
        <p class="title">{{wallType[id].name}}</p>
        <p class="slogan">{{wallType[id].slogan}}</p>
        <div class="labels" v-if="id!=2">
            <p class="label-item" :class="{labelselected:nlabel==-1}" @click="selectLabel(-1)">全部</p> <!--做一个动态class去生，并写函数点击-->
            <p class="label-item" :class="{labelselected:nlabel==index}" v-for="(e,index) in label[id]" :key="index" @click="selectLabel(index)">{{e}}</p>
        </div>

        <!--主视觉加区分-->
        <div class="card" :style="{width:nWidth+'px'}" v-show="id==0"> <!--id=0时显示留言墙-->
            <word-card-vue v-for="(e,index) in cards" :key="index" :word="e" class="card-inner" :width="'288px'" :class="{cardselected: index==cardSelected}" @toDetail="selectedCard(index)"></word-card-vue>  <!--click => toDetail-->
        </div>
        <div class="photo" v-show="id==1">
            <!--先静态-->
            <!--img :src="require('../../static/'+photo[0].imgurl+'.webp')" /-->
            <photo-card-vue :photo="e" class="photo-card" v-for="(e,index) in cards" :key="index" @toDetail="selectedCard(index)"></photo-card-vue> <!--photo直接传给下层props-->
        </div> <!--id=1时显示照片墙-->

        <!--卡片状态-->
        <div class="none-msg" v-if="isLoading==0"> <!--无卡片时的。可否并？-->
            <img :src="none[id].url" />   <!--照片和留言不同的-->
            <p>{{ none[id].msg }}</p>
        </div>

        <!--加载状态-->
        <div class="loading" v-show="isLoading==-1">
            <div id="lottie"></div> <!--id关联的，需要函数-->
            <p>加载中...</p>
        </div>

        <!--到底-->
        <p class="bottom-tip" v-show="isLoading==2">已到底</p> <!--与getWall对应，还是需要单独的判断-->

        <div class="add" :style="{bottom:addBottom+'px'}" @click="addCard" v-show="id!=2 && !modal"> <!--窗口打开时，不显示此按钮-->
            <span class="iconfont icon-tianjia"></span>
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
        <ct-modal-vue :title="title" @close="closeViewer" :isModal="modal">
            <new-card-vue :id="id" @addClose="changeModal" v-if="cardSelected==-1" @clickbt="newCard"></new-card-vue> <!--暂时接消息关闭，还会补充接消息传数据；同时，还需要，区分新增和详情，-1时才开newcard-->
            <card-details-vue v-if="cardSelected!=-1" :card="cards[cardSelected]" @cardDeleted="cardDeleted" :id="id"></card-details-vue> <!--1时才开，并把对应的card信息传给底层--> <!--改原先的word为cards[cardSelected]-->
        </ct-modal-vue> <!--作变量-->
        <ct-viewer-vue :isView="view" :photos="photoArr" :currentNumber="cardSelected" @viewSwitch="viewSwitch"></ct-viewer-vue> <!--让之也接受modal的控制，详情界面出来再出来--> <!--cardSelected中存的就是当前选择的卡片-->
        <!--接消息viewSwitch，则增减currentNumber-->

        <!--回忆录-->
        <ct-memoir-vue v-show="id==2" :id="id" @hasMemory="NoMemory"></ct-memoir-vue>

        <!--加入班级-->
        <join-class-vue v-show="isJoining==1" @closeJoin="closeJoin"></join-class-vue>

        <!--新建班级-->
        <new-class-vue v-show="isCreating==1" @closeCreate="closeCreate"></new-class-vue>

        <!--班级管理-->
        <manage-class-vue v-show="isManaging==1" @closeManage="closeManage"></manage-class-vue>

    </div>
</template>

<script>
import {wallType, label, none} from '@/utils/data'
import wordCardVue from '@/components/WordCard.vue'
// import { photo } from "../../mock/index"
import ctModalVue from '@/components/CTModal.vue'
import newCardVue from '@/components/NewCard.vue'
import cardDetailsVue from '@/components/CardDetails.vue'
import photoCardVue from '@/components/PhotoCard.vue'
import ctViewerVue from '@/components/CTViewer.vue'
import lottie from 'lottie-web'
import loading from '@/assets/images/loading.json'
import { findWallPageApi }  from '@/api/index'
import ctMemoirVue from '@/components/CTMemoir.vue'
import joinClassVue from '@/components/JoinClass.vue'
import newClassVue from '@/components/NewClass.vue'
import manageClassVue from '@/components/ManageClass.vue'

export default {
    data() {
        return {
            wallType,
            label,
            none,
            // 现在用动态的了 id:0, //用之控制留言墙和照片墙的切换
            nlabel: -1, // 当前对应的标签
            // word: '',  // 长度为0时也有特殊显示
            cards: [], 
            // photo: photo.data,
            photoArr: [], // 图片列表
            nWidth: 0, // 卡片模块宽度
            addBottom: 50, // add按钮的bottom距离变量
            title: '写留言', // 新建标题
            modal: false, // 是否调用弹窗
            view: false, // 大图
            cardSelected:-1, // 当前选择卡片，没有选择时是-1，有选择才是对应的数字
            isLoading: -1, // 加载状态，-1是加载状态，0为没拿到数据
            // user: '', // 也需要userid
            page: 1, // 当前page从1开始
            pagesize: 15, 
            //memory: 1,
            hasmemory: false,
            isJoining: 0,
            isCreating: 0,
            isManaging: 0,
        }
    },
    components: {
        wordCardVue,
        ctModalVue,
        newCardVue,
        cardDetailsVue,
        photoCardVue,
        ctViewerVue,
        ctMemoirVue,
        joinClassVue,
        newClassVue,
        manageClassVue
    },
    computed: {
        // 动态：控制留言墙和照片墙的切换
        id() {
            return this.$route.query.id;
        },
        user() {
            return this.$store.state.user;
        },
        classId() {
            return this.$store.state.classId;
        }
    },
    watch: {  // 监听id的变化
        id() {
            this.modal = false;
            this.view = false;
            this.nlabel = -1;  // 选择全部，后续还要加读取数据的变化
            this.cardSelected = -1;
            this.cards = [];
            this.page = 1; // 从第一页开始
            this.getWallCard(this.id);  // 重新装载一下
            // this.memory = 0;
        },
        classId() {
            this.modal = false;
            this.view = false;
            this.nlabel = -1;  // 选择全部，后续还要加读取数据的变化
            this.cardSelected = -1;
            this.cards = [];
            this.page = 1; // 从第一页开始
            this.getWallCard(this.id);  // 重新装载一下
        }
    }, 
    created() {
        this.cardWidth();
        // this.getPhoto();
    },
    mounted() {
        this.loading(); // 启动
        this.getUser();
        this.user = this.$store.state.user;
        this.classId = this.$store.state.classId;
        // 窗口resize时候触发函数handleResize
        window.addEventListener('resize', this.cardWidth);
        // 监听滚动
        window.addEventListener('scroll', this.scrollBottom);
    },
    unmounted() { // 注销
        window.addEventListener('resize', this.cardWidth);
        window.addEventListener('scroll', this.scrollBottom);
    },
    methods: {
        // 切换标签 - 清空再重装
        selectLabel(e) {
            this.nlabel = e;
            this.cards = [];
            this.page = 1; // 从第一页开始
            this.getWallCard(this.id);
        },
        // card的宽度动态监听
        cardWidth() {
            let wWidth = document.body.clientWidth; // 获取整个页面的宽度
            this.nWidth = Math.floor((wWidth-120)/300)*300; // 120为左右整体安全距离，300为均分宽度（288+6+6），再向下取整，最后乘300就是最终宽度
        },
        // 页面滚动监听
        scrollBottom() {
            // 距离顶部的高度
            let scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
            // 屏幕高度
            let clientHeight = document.documentElement.clientHeight;
            // 内容高度
            let scrollHeight = document.documentElement.scrollHeight;
            if (scrollTop+clientHeight+230 >= scrollHeight) { // 底部高度为200
                this.addBottom = scrollTop+clientHeight+230-scrollHeight;
            } // 当滚动到footbar刚好进入时，30要变大了
            else {
                this.addBottom = 30;
            }

            // 加载更多
            if (scrollTop + clientHeight == scrollHeight) {  // 已经到底
                this.getWallCard(this.id);
            }
        },

        // 新建card
        addCard() {
            this.title="写留言";
            this.changeModal();
        },

        // 更改弹窗状态
        changeModal() {
            if (this.modal == true) {
                this.cardSelected = -1;  // 防止关闭后，还有框
            }
            this.modal = !this.modal;
        },

        // 更改弹窗状态 for viewer
        closeViewer() {
            if (this.modal == true) {
                this.cardSelected = -1;  
            }
            this.modal = !this.modal;
            if (this.id == 1) {
                this.view = false;
            }
        },

        // 选择卡片 - 要多做一个判断了
        selectedCard(e) {
            this.title="";
            if (e!=this.cardSelected) {
                this.cardSelected = e;
                this.modal=true;
                if (this.id == 1) {
                    this.view = true;
                }
            }
            else {
                this.cardSelected = -1;  // 复点可取消
                this.modal=false;
                if (this.id == 1) {
                    this.view = false;
                }
            }
        },
        // 获取图片索引 可以不要，直接在getWall里面渲染
        getPhoto() {
            for (let i=0; i<this.cards.length; i++) {
                this.photoArr.push(this.cards[i].imgurl)
            }
            // console.log(this.photoArr);
        }, 
        // 切换图片消息接收
        viewSwitch(e) {
            if (e == 0) { // 向左
                this.cardSelected--;  // 边界在viewer中进行处理
            }
            else {
                this.cardSelected++;
            }
        },

        // 留言插入处理
        newCard(e) {
            if (this.cards.length < 1) {
                this.cards = [];
                this.page = 1; // 从第一页开始
                this.getWallCard(this.id);
            }
            else {
                // console.log(e)
                this.cards.unshift(e);  // 报错？需要重整一下数据！
                // this.getPhoto();  // 在最后了
            }
            this.changeModal(); // 关闭弹窗
            this.photoArr.unshift(e.imgurl);
        },

        // 删除卡片
        cardDeleted() {
            this.changeModal(); // 关闭弹窗
            this.view = false; // 关闭大图
            this.cards = [];
            this.page = 1; // 从第一页开始
            this.getWallCard(this.id);
        },
        
        // 加载动画
        loading() {
            if (this.isLoading == -1) {
                this.$nextTick(async () => {
                    var params1 = {
                        container: document.getElementById("lottie"),
                        renderer: "svg",
                        loop: true,
                        autoplay: true,
                        animationData: loading,
                    };
                    lottie.loadAnimation(params1);
                })
            }
        },
        
        // 获取卡片
        getWallCard(id) {
            if (id == 2) {
                if (this.hasmemory) {
                    this.isLoading = 0;
                    console.log(111)
                }
                else {
                    this.isLoading = 1;
                    console.log(222)
                }
            }
            else {
                if (this.page > 0) {
                    this.isLoading = -1;  // 先加载动画
                    let data = {
                        type: id,
                        page: this.page,
                        pagesize: this.pagesize,
                        userId: this.user.id, // 点赞的匹配
                        label: this.nlabel,
                        classId: this.classId, // 班级的匹配
                    }
                    // console.log(data)
                    findWallPageApi(data).then((res) => {
                        this.cards = this.cards.concat(res.message);
                        // 设置下一次的page
                        // console.log(res.message)
                        if (res.message.length) {  // 有数据
                            this.page++;
                        }
                        else {
                            this.page = 0; // 防止继续自增 - 可以与触底对应
                        }
                        if (this.cards.length > 0) {
                            this.isLoading = 1;  // 有数据
                            if (this.page == 0) { // 有数据，但已经读完
                                this.isLoading = 2; // 还是要的，相当于完成读取了
                            }
                        }
                        else {
                            this.isLoading = 0;  // 没有数据
                        }
                        // 图片单独拿出来，为了方便看大图使用
                        if (this.id == 1) {
                            for (let i=0; i<this.cards.length; i++) {
                                this.photoArr.push(this.cards[i].imgurl)
                            }
                        }
                    });
                }    
            }

        },
        getUser() {
            let timer = setInterval(()=> {
                if (this.user) {
                    clearInterval(timer)
                    this.getWallCard(this.id)
                }
            }, 10)
        },
        NoMemory() {
            this.isLoading = 0;
            this.hasmemory = true;
            console.log(this.isLoading)
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
    }
}
</script>

<style lang="less" scoped>
.wall-message {
    min-height:700px; 
    padding-top: 52px;
    .title {
        padding-top: 48px;
        padding-bottom: @padding-8;
        font-size: 56px;
        color: @gray-0;
        text-align: center;
        font-weight: 600;
    }
    .slogan {
        color: @gray-1;
        text-align: center;
    }
    .labels {
        display: flex; // 平铺
        justify-content: center; // 居中
        margin-top: 40px;
        .label-item {
            padding: 0 14px;
            display: flex;
            align-items: center; // 上下居中
            height: 30px;
            margin: @padding-4;
            color: @gray-1;
            box-sizing: border-box;
            cursor: pointer; // 形成点击
        }
        .labelselected { // 选中样式
            color: @gray-0;
            font-weight: 400;
            border: 1px solid @gray-1;
            border-radius: 16px;
        }
    }
    .card {
        display: flex;
        flex-wrap: wrap; // 换行
        // justify-content: center; // 居中，但会导致少量元素也会居中
        padding-top: 28px; // 顶部间距
        margin: auto; // 居中
        .card-inner {
            margin: 6px;
        }
        .cardselected {
            border: 1px solid @primary-color;
        }
    }
    .photo {
        width: 88%;
        margin: 0 auto; // 自动居中
        columns: 6; // 6列
        column-gap: @padding-4; // 左右间距
        padding-top: 28px;
    }
    .photo-card {
        // width: 200px; 仅仅只是调试时用的限定
        margin-bottom: @padding-4; // 上下间距
        break-inside: avoid; // 防止换行错乱
    }
    .add {
        width: 56px;
        height: 56px;
        background: @gray-0;
        box-shadow: 0px 4px 8px 0px rgba(0,0,0,0.08);
        border-radius: 28px;
        position: fixed;
        right: 30px;
        display: flex;
        justify-content: center;
        align-items: center;
        transition: @tr; // 渐变动画时长
        .icon-tianjia {
            color: @gray-3;
            font-size: 24px;
        }
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
    .none-msg {
        width: 100%;
        text-align: center;
        padding-top: 80px;
        position: absolute;
        top: 280px;
        img {
            display: inline;
            width: 100px;
        }
        p {
            font-family: wordF;
            font-weight: 600;
            font-size: 20px;
            line-height: 75px;
            color: @gray-2;
        }
    }
    .loading {
        text-align: center;
        width: 100%;
        p {
            margin-top: 20px;
            font-family: wordF;
            font-weight: 500;
            font-size: 20px;
            color: @gray-1;
        }
    }
    #lottie {
        margin-top: 40px;
        height: 100px;
    }
    .bottom-tip {
        text-align: center;
        font-family: wordF;
        font-weight: 500;
        font-size: 20px;
        color: @gray-2;
        padding: 20px;
    }
}

</style>