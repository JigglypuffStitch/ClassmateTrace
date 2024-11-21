<template>
    <div class="cr-m-2">
        <div class="ct-memoir">
            <div class="memory" v-for="(memory, index) in filteredMemories" :key="index">
                <div class="memory-card">
                    <img :src="memory.items[0].imgurl" class="memory-img"/>
                    <div class="memory-bg" @click="openView(index)"></div> 
                    <p @click="openView" class="memory-title">{{memory.title}}</p>
                </div>
            </div>
        </div>
        <transition name="memoir">
            <div class="memoirview" v-show="show == 1">
                <memoir-view-vue @viewClosed="closeView" :items="items" :name="name"></memoir-view-vue>
            </div>
        </transition>
    </div>
</template>

<script>
import memoirViewVue from '@/components/MemoirView.vue'
import { findLabelCountsApi, findLabelItemsApi } from "@/api/index"
import { label } from '@/utils/data'
import { baseUrl } from '@/utils/env'  // 图片地址 
export default {
    data() {
        return {
            show: 0,
            classId: this.$store.state.classId,
            plabel: label[1],
            baseUrl: baseUrl,
            memories: [],
            items: [],
            name: '',
            memorySelected: -1,
            /*
            items: [
                { img: require('/static/0.webp'), name: "111", des: "Tinh ru anh di chay pho, chua kip chay pho thi anhchay mat tieu" },
                { img: require('/static/1.webp'), name: "222", des: "小窝里的爱" },
                { img: require('/static/2.webp'), name: "333", des: "在小相馆中" },
                { img: require('/static/3.webp'), name: "444", des: "Tinh ru anh di chay pho, chua kip chay pho thi anhchay mat tieu" },
                { img: require('/static/4.webp'), name: "555", des: "Tinh ru anh di chay pho, chua kip chay pho thi anhchay mat tieu" },
                { img: require('/static/5.webp'), name: "666", des: "Tinh ru anh di chay pho, chua kip chay pho thi anhchay mat tieu" }
            ],
            name: '回忆1'*/
        }
    },
    computed: {
        filteredMemories() {
            // 返回 items 存在并且有内容的 memories
            return this.memories.filter(memory => memory.items && memory.items.length > 0);
    }
    },
    props: {
        id: {
            default: 0,
        }
    },
    components: {
        memoirViewVue
    },
    watch: {  // 监听id的变化
        id() {
            if (this.id != 2) {
                this.closeView();
            }
            else {
                if (this.memories) {
                    // this.$emit("hasMemory");
                }
            }
        },
        memories() {
            if (this.memories) {
                // this.$emit("hasMemory");
            }
        }
    },
    methods: {
        closeView() {
            this.show = 0;
            // console.log(this.show)
        },
        openView(e) {
            this.show = 1;
            this.memorySelected = e;
            this.items = this.memories[this.memorySelected].items;
            this.name = this.memories[this.memorySelected].title;
        },
        createMemoir() {
            let data = {
                classId: this.classId,
                type: 1
            }

            findLabelCountsApi(data).then((res) => {
                // 筛选出 count 值大于等于 10 的 label
                let filteredLabels = res.message
                    .filter(item => item.count >= 5)
                    .map(item => item.label);

                // 检查 filteredLabels 是否为空
                if (filteredLabels.length === 0) {
                    this.memories = []; // 若为空，直接设为空数组
                    console.log("No labels with count >= 5 found. memories is set to an empty array.");
                    this.$emit("hasMemory");
                    return;
                }

                // 创建一个接口调用的 Promise 数组，每个 label 调用一次 findLabelItemsApi
                let promises = filteredLabels.map(label => {
                    return findLabelItemsApi({
                        classId: this.classId,
                        type: 1,
                        label: label
                    }).then(items => {
                        // 将每个 label 的数据按指定格式存储
                        return {
                            items: items.message,
                            title: label>7?items.message[0].name:this.plabel[label]
                        };
                    });
                });

                // 使用 Promise.all 等待所有接口调用完成后存储到 this.memories
                Promise.all(promises).then((results) => {
                    this.memories = results;
                    console.log(this.memories); // 输出最终的 memories 数组
                }).catch(error => {
                    console.error("Error fetching label items:", error);
                });
            });
        }
    },
    created() {
        this.createMemoir();
    }
}
</script>

<style lang="less" scoped>
.memoir {
    &-enter { // 入场动画，这里只变化透明度
        &-from { 
            opacity: 0;
        }
        &-active { 
            transition: all 0.3s ease-out;
        }
        &-to {
            opacity: 1;
        }
    }
    &-leave { // 出场动画
        &-from {
            opacity: 1;
        }
        &-active {
            transition: all 0.3s ease-in;  // 慢速开始
        }
        &-to {
            opacity: 0;
        }
    }
}
.cr-m-2 {
    min-height:400px; 
    padding-top: 52px;
}

.ct-memoir {
    width: 88%;
    margin: 0 auto; // 自动居中
    columns: 4; // 6列
    column-gap: @padding-12; // 左右间距
    padding-top: 28px;
}

.memory {
    //width: 200px;
    margin-bottom: @padding-4; // 上下间距
    break-inside: avoid; // 防止换行错乱
    position: relative; // 每张图片要定位
        
    .memory-img {
        width: 100%; // 结合上层的width固定一个大小
        border-radius: 20px;
        // box-shadow:  0px 0px 12px #757474;
    }
    .memory-bg {
        position: absolute; // 有上下层关系
        top: 0;
        left: 0;
        background: rgba(0,0,0,0.2);
        border-radius: 20px;
        // background: red;  // 用于调试
        width: 100%;
        height: 100%;
        opacity: 0; // hover时变成1
        transition: @tr; // 缓动
        cursor: pointer;
    }
    .memory-title {
        position: absolute;
        left: @padding-20;
        top: @padding-20;
        font-family: wordF;
        font-weight: 600;
        font-size: 20px;
        color: @gray-3;
        opacity: 0;
        cursor: pointer;
    }
    &:hover {
        .memory-bg {
            opacity: 1;
        }
        .memory-title {
            opacity: 1;
        }
    }
}



</style>