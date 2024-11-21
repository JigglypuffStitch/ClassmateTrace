<template>
    <div class="ct-photo-card">
        <img :src="photo.imgurl" class="photo-img"/> <!--只用photo，不用photo[0]，因为是单张了；注意。src！！！！！-->
        <div class="photo-bg" @click="toDetail"></div> <!--蒙版，这里click！-->
        <div class="photo-like" @click="clickLike">
            <span class="iconfont icon-aixin1" :class="{islike:card.islike[0].count>0}"></span>
            <span class="like-data">{{ photo.like[0].count }}</span>
        </div>
    </div>
</template>

<script>
import { label, cardColor } from '@/utils/data'
import { baseUrl } from '@/utils/env'  // 图片地址 
import { insertFeedbackApi, albumApi } from '@/api/index'
export default {
    data() {
        return {
            label,
            cardColor,
            baseUrl,
            user: this.$store.state.user,
        }
    },
    props: {
        photo: { 
            default: {},
        },
    },
    computed: {
        card() {
            return this.photo;
        }
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
                    this.insertPhotoLike(data.wallId);
                })
            }
        },
        async insertPhotoLike(e) {
            let data = {
                pictureId: e,
            }
            const response = await albumApi.likePicture(data);
            console.log(response);
        }
    },
    created() {
        // console.log(this.card);
    }
}
</script>

<style lang="less" scoped>
.ct-photo-card {
    position: relative; // 每张图片要定位
    .photo-img {
        width: 100%; // 结合上层的width固定一个大小
    }
    .photo-bg {
        position: absolute; // 有上下层关系
        top: 0;
        left: 0;
        background: rgba(0,0,0,0.2);
        // background: red;  // 用于调试
        width: 100%;
        height: 100%;
        opacity: 0; // hover时变成1
        transition: @tr; // 缓动
        cursor: pointer;
    }
    .photo-like {
        position: absolute;
        left: @padding-8;
        top: @padding-8;
        background: rgba(255,255,255,0.80);
        border-radius: 20px;
        height: 28px;
        padding: 0 10px;
        display: flex;
        align-items: center; // 上下居中
        backdrop-filter: blur(10px);
        opacity: 0; // hover时变成1
        transition: @tr; // 缓动
        cursor: pointer; // 可点赞
        .icon-aixin1 {
            color: @gray-2;
            padding-right: @padding-4;
        }
        .islike {
            color: @like-color;
        }
        .like-data {
            color: @gray-0;
        }
    }
    &:hover {
        .photo-bg {
            opacity: 1;
        }
        .photo-like {
            opacity: 1;
        }
    }
}
</style>