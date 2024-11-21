<template>
    <div class="overlay"></div>
    <div class="d-container">
        <div id="slide">
            <div v-for="(item, index) in items" :key="index" class="item" :style="{ backgroundImage: `url(${item.imgurl})` }">
                <div class="content">
                <div class="name">{{ item.name }}</div>
                <div class="des">{{ item.message }}</div>
                </div>
            </div>
        </div>

        <div class="buttons">
            
            <button id="prev"><span class="iconfont icon-xiangzuo"></span></button>
            <button id="next"><span class="iconfont icon-xiangyou"></span></button>
        </div>

        <button class="close-btn" @click="closeView"></button>
        <h3 class="d-title">回忆录<span>｜{{name}}</span></h3>
    </div>
</template>

<script>
import { baseUrl } from '@/utils/env' 
export default {
    data() {
        return {
            baseUrl,
        }
    },
    props: {
        items: {  // 上级传进来是数组
            default: [],
        }, 
        name: {
            default: '',
        }
    },
    mounted() {
        document.getElementById('next').onclick = () => {
        let lists = document.querySelectorAll('.item');
        document.getElementById('slide').appendChild(lists[0]);
        };
        document.getElementById('prev').onclick = () => {
        let lists = document.querySelectorAll('.item');
        document.getElementById('slide').prepend(lists[lists.length - 1]);
        };
    },
    methods: {
        closeView() {
            this.$emit('viewClosed');
            //console.log('ssss')
        }
    }
}
</script>

<style lang="less" scoped>
.overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.708); /* 半透明黑色遮罩 */
    z-index: 0; /* 遮罩层的层级 */
}
.d-container{
    position: fixed;
    left:50%;
    top:52%;
    transform: translate(-50%,-50%);
    width:1000px;
    height:550px;
    padding:50px;
    background-color: #f5f5f5;
    box-shadow: 0 0px 80px #a29f9f; // 3e3d3d
}
#slide{
    width:max-content;
    margin-top:50px;
}
.item{
    width:200px;
    height:300px;
    background-position: 50% 50%;
    display: inline-block;
    transition: 0.5s;
    background-size: cover;
    position: absolute;
    z-index: 31;
    top: 50%;
    transform: translate(0,-50%);
    border-radius: 20px;
    box-shadow:  0 30px 50px #505050;
}
.item:nth-child(1),
.item:nth-child(2){
    left:0;
    top:50;
    transform: translate(0,-50%);
    border-radius: 0;
    width:100%;
    height:100%;
    box-shadow: none;
}

.item:nth-child(3){
    left:50%;
}
.item:nth-child(4){
    left:calc(50% + 220px);
}
.item:nth-child(5){
    left:calc(50% + 440px);
}
.item:nth-child(n+6){
    left:calc(50% + 660px);
    opacity: 0;
}

.item .content{
    position: absolute;
    top:50%;
    left:100px;
    width:300px;
    text-align: left;
    padding:0;
    color:#eee;
    transform: translate(0,-50%);
    display: none;
}
.item:nth-child(2) .content{
    display: block;
    z-index: 32;
}
.item .name{
    font-size: 35px;
    font-weight: bold;
    text-shadow: 0px 2px 8px rgba(0, 0, 0, 0.8);
    opacity: 0;
    animation:showcontent 1s ease-in-out 1 forwards
}
.item .des{
    margin:20px 0;
    text-shadow: 0px 2px 8px rgba(0, 0, 0, 0.8);
    opacity: 0;
    animation:showcontent 1s ease-in-out 0.3s 1 forwards
}
@keyframes showcontent{
    from{
        opacity: 0;
        transform: translate(0,100px);
        filter:blur(33px);
    }to{
        opacity: 1;
        transform: translate(0,0);
        filter:blur(0);
    }
}

.buttons{
    position: absolute;
    right:0%;
    bottom: 30px;
    z-index: 33;
    text-align: center;
    width:100%;
}
.buttons button{
    width:50px;
    height:50px;
    border-radius: 50%;
    border:0px solid #FFFFFF;
    background-color: @gray-3;
    transition: 0.5s;
    cursor: pointer;
    margin: 0 15px;
}
.buttons button:hover{
    background-color: @gray-2;
}


.d-title{
    position: absolute;
    top: 1%;
    right: 4%;

    z-index: 34;

    color:#eee;
    font-size: 1.2rem;
    text-transform: uppercase;
    font-weight: 600;
    letter-spacing: 2px;
    text-shadow: 0px 0px 8px rgba(0, 0, 0, 1);
}
.d-title span{
    font-weight: 450;
    text-shadow: 0px 0px 10px rgba(0, 0, 0, 1);
}

.close-btn {
    position: absolute;
    top: 3%; /* 匹配swiper-button-prev的位置 */
    left: 3%; /* 匹配swiper-button-prev的位置 */
    z-index: 34; /* 确保在Swiper按钮之上 */
    
    width: 30px;
    height: 30px;
    background-color: transparent;
    border: none;
    cursor: pointer;
}
.close-btn::before, .close-btn::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 25px;
    height: 2.5px;
    background-color: #eee;
    transform: translate(-50%, -50%) rotate(45deg);
}
.close-btn::after {
    transform: translate(-50%, -50%) rotate(-45deg);
}
</style>