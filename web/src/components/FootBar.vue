<template>
    <div class="foot-bar">
        <div class="foot-bar-inner">
            <div class="foot-left">
                <div class="logo">
                    <img src="@/assets/images/classmaps_logo_b.png" class="logo-img"/>
                    <p class="logo-name">同窗行迹</p>
                </div>
                <p class="top-p">同窗行迹，你的电子同学录、回忆录与蹭饭图，</p>
                <p class="top-p">旨在为学生群体提供一个智能化、电子化、多样化、集成化的毕业纪念电子系统，让不同年岁中的班级记忆伴随你一路成长。</p>
                <p class="statement"><span>前端制作</span><span>史非</span></p>
            </div>
            <div class="link">
                <p class="title">链接</p>
                <div class="link-inner">
                    <a href="www.huohuo90.com" target="_blank" class="link-name">github源码</a> <!--_blank表打开新窗口-->
                    <a href="www.huohuo90.com" target="_blank" class="link-name">设计图纸</a> 
                </div>
            </div> 
            <div class="foot-right">
                <p class="title">联系</p>
                <div class="right-inner">
                    <div>
                        <img src="@/assets/images/wx@2x.jpg"/>
                        <p class="contact-title">微信</p>
                    </div>
                    <div>
                        <img src="@/assets/images/qq@2x.jpg"/>
                        <p class="contact-title">QQ</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {

}
</script>

<style lang="less" scoped>
.foot-bar {
    width: 100%;
    height: 200px;
    background: @gray-0;
    padding: 20px;
    box-sizing: border-box;
    margin-top: 40px; 
    .foot-bar-inner {
        width: 1200px;
        margin: 0 auto;  
        display: flex;
        justify-content: space-between;
        p{
            color: rgba(255,255,255,0.5);
        }
    }
    .foot-left {
        .logo {
            display: flex; // 居中，从上至下排列
            align-items: center;
            margin-bottom: 24px;
            .logo-img {
                width: 27px;
                height: 27px;
            } 
            .logo-name {
                font-size: 20px;
                color: @gray-4;
                font-weight: 400;
                padding-left: 10px;
            }
        }
        .top-p{
            font-size: @size-12;
            padding-bottom: 4px;
            width: 400px;
        }
        .statement{
            font-size: @size-12;
            padding-top: 12px;
            span {
                padding-right: 20px;
            }
        }
    }
    .title {
        font-size: @size-16;
        padding-bottom: 12px;
    }
    .link {
        padding: 0 40px;
        flex: 1;
        .link-inner {
            a {
                color: rgba(255,255,255,0.5);
                padding-right: 20px;
                font-size: @size-12;
            }
        }
    }
    .foot-right {
        .right-inner {
            display: flex;
            img {
                width: 100px;
                padding-right: 40px;
            }
            .contact-title {
                font-size: @size-12;
                padding-top: 8px;
            }
        }
    }

}
</style>