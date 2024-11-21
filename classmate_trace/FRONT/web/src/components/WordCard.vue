<template>
     <div class="ct-word-card" :style="{width:width,background: cardColor[card.color]}"> <!--写ct-word-card以与messagewall中区分；要修改的样式都放到style里去，对应card的一个参数！-->
        <div class="top">
            <p class="time">{{ dataOne(card.moment)}}</p>
            <p class="label">{{ label[card.type][card.label] }}</p>
        </div>
        <p class="words" @click="toDetail">{{ card.message }}</p>
        <div class="foot">
            <div class="foot-left">
                <div class="intericon" @click="clickLike"> <!--进一步区分，以方便定义间隔，但有两个一致的平行组-->
                    <span class="iconfont icon-aixin1" :class="{islike:card.islike[0].count>0}"></span> <!--引icon-->
                    <span class="value">{{ card.like[0].count }}</span> <!--与拿到的数据格式匹配-->
                </div>
                <div class="intericon" v-show="card.comcount[0].count>0"> <!--有留言才显示-->
                    <span class="iconfont icon-liuyan"></span>
                    <span class="value">{{ card.comcount[0].count }}</span>
                </div>
            </div>
            <div class="name">{{ card.name }}</div>
        </div>
     </div>
</template>

<script>
import { label, cardColor } from '@/utils/data'
import { dataOne } from '@/utils/methods'
import { insertFeedbackApi, commentApi } from '@/api/index'
export default {
    data() {
        return {
            label,
            cardColor,
            dataOne,
            user: this.$store.state.user,
        }
    },
    props: {
        width: { 
            default: '100%',// 默认宽度
        },
        word: { 
            default: {},
        }
    },
    computed: {
        card() {  // 实际数据渲染
            return this.word;
        }
    },
    created() {
        // console.log(this.card);  // 打印出数据，方便针对性地设计
    },
    methods: {
        // 显示详情
        toDetail() {
            this.$emit('toDetail')  // 传消息很方便的！
        },
        // 点击喜欢
        clickLike() {
            // 如果点过则不能点了
            if (this.card.islike[0].count==0) {
                let data = {
                    wallId: this.card.id,
                    userId: this.user.id,  // user还要拿
                    type: 0,  // 0是喜欢
                    moment: new Date(),
                }
                insertFeedbackApi(data).then(() => {
                    // 反馈到界面
                    this.card.like[0].count++;  // 数量
                    this.card.islike[0].count++; // 样式
                    this.$message({ type: "success", message: "点赞成功" });
                    this.insertMessageLike(data.wallId);
                })
            }
        },
        async insertMessageLike(e) {
            let data = {
                commentId: e,
            }
            const response = await commentApi.likeComment(data);
            console.log(response)
        }
    }
}
</script>

<style lang="less" scoped>

.ct-word-card {
    height: 240px; // 高度一定
    padding: 10px 20px;
    box-sizing: border-box;
    position: relative;
    .top {
        display: flex;
        justify-content: space-between; // 两边分
        padding-bottom: 16px;
        p { 
            font-size: @size-12;
            color: @gray-2;
        }
    }
    .words {
        height: 140px;
        font-family: wordF;
        font-size: 15px;
        color: @gray-0;
        cursor: pointer;
    }
    .foot {
        display: flex;
        justify-content: space-between; // 均分
        position: absolute;
        bottom: 16px;
        left: 0;
        padding: 0 @padding-20;
        box-sizing: border-box;
        width: 100%;
        .foot-left {
            display: flex;
            .value{
                font-size: @size-12;
                color: @gray-2;
                line-height: 16px;
                padding-left: @padding-4;
            }
            .iconfont {
                font-size: 16px; // 可以直接当font用
                color: @gray-2;
            }
            .intericon {
                padding-right: @padding-8;
                display: flex;
                align-items: center; // 要上下居中
            }
            .icon-aixin1 {
                cursor: pointer;
                transition: @tr;
                &:hover { // 悬浮变色
                    color: @like-color;
                }
            }
            .islike {
                color: @like-color;
            }
            
        }
        .name {
            font-family: wordF;
            font-size: 16px;
            color: @gray-1;
            font-weight: 400;
        }
    }
}
</style>