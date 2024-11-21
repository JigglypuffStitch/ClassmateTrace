<template>
    <div class="top-bar">
        <div class="logo">
            <img src="@/assets/images/classmaps_logo_c.png" class="logo-img"/>
            <p class="logo-name">同窗行迹</p>
            <p class="part-name"></p>
        </div>
        <div class="menu">
            <!--静变动，根据id的0和1来判断-->
            <ct-buttons-vue :nom="$route.path === '/map'?'c_primary':'c_secondary'" class="menu0" @click="goToMap">蹭饭图</ct-buttons-vue>
            <ct-buttons-vue :nom="id==0?'c_primary':'c_secondary'" class="menu1" @click="changeWall(0)">留言墙</ct-buttons-vue>
            <ct-buttons-vue :nom="id==1?'c_primary':'c_secondary'" class="menu2" @click="changeWall(1)">照片墙</ct-buttons-vue>
            <ct-buttons-vue :nom="id==2?'c_primary':'c_secondary'" class="menu3" @click="changeWall(2)">回忆录</ct-buttons-vue>
            <ct-buttons-vue :nom="$route.path === '/gathering'?'c_primary':'c_secondary'" class="menu4" @click="goToMeeting">小聚厅</ct-buttons-vue>
            <ct-buttons-vue :nom="$route.path === '/personalSpace'?'c_primary':'c_secondary'" @click="goToPS">同学圈</ct-buttons-vue>
        </div>
        <div class="user">
            <span class="cticon icon--_shezhi" @click="openSetting" :class="{'onset': $route.path==='/setting'}"></span>
        </div>
    </div>
</template>

<script>
import CtButtonsVue from './CTButtons.vue' // 引用buttons文件
export default {
    data() {
        return {
            // isSetting: false,
            // backTrace: '',
        }
    },
    components: {
        CtButtonsVue,
    },
    computed: { // 动态获取的
        id() {  // 获取id
            return this.$route.query.id; // 改网址，可以获取
        },
    },
    methods: {
        // 切换墙
        changeWall(e) {
            this.$router.push({  // 让它去赋值
                path: '/wall',
                query: { id: e },
            })
        },
        goToMap() {
            this.$router.push('/map')
        },
        goToMeeting() {
            this.$router.push('/gathering')
        },
        openSetting() {
            if (this.$route.path!='/setting') {
                const tmp = this.$route.path;
                this.$router.push('/setting'); // 跳转设置页面
                sessionStorage.setItem('backTrace', tmp); // 存储路径
                
            }
            else {
                const backTrace = '/map'; // 获取路径
                this.$router.push(backTrace); // 回到原路径
                sessionStorage.removeItem('backTrace'); // 清除路径
            }
            
        },
        goToPS() {
            this.$router.push('/personalSpace')
        }
    }
}
</script>

<style lang="less" scoped>
.top-bar {
    width: 100%;
    height: 52px;
    background: rgba(255,255,255,0.80);
    box-shadow: 0px 0px 4px 0px rgba(0,0,0,0.1);
    backdrop-filter: blur(10px); // 毛玻璃效果
    position: fixed; // 绝对定位，让其位置是固定的
    top: 0;
    left: 0;
    z-index: 9999; // 最顶层
    display: flex;
    justify-content: space-between; // 左中右三分
    align-items: center; // 上下文居中
    padding: 0 30px; // 左间距30px
    box-sizing: border-box; // 边界也算入宽高
    .logo {
        width: 400px; // 赋予宽度，让左右同宽，中间便可居中
        display: flex;
        align-items: center;
        .logo-name {
            font-size:20px;
            color: @gray-0;
            font-weight: 400;
            padding-left: 10px; // 左侧距离
        }
        .part-name {
            font-size:20px;
            color: @gray-0;
            font-weight: 300;
        }
    }
    .menu {
        .menu0 {
            margin-right: 24px;
        }
        .menu1{
            margin-right: 24px;
        }
        .menu2{
            margin-right: 24px;
        }
        .menu3{
            margin-right: 24px;
        }
        .menu4{
            margin-right: 24px;
        }
    }
    .user {
        width: 400px; // 赋予宽度，让左右同宽，中间便可居中
        .icon--_shezhi {
            float: right; // 居右
            color: @gray-1;
            font-size: 36px;
            cursor: pointer;
        }
        .onset {
            color: @primary-color;
        }
    }
}
.logo-img {
    width: 27px;
    height: 27px;
}
</style>