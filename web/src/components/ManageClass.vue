<template>
    <transition name="manage">
        <div class="modal-overlay" @click.self="close">
        <div class="modal">
            <div class="modal-header">
                <span>班级管理</span>
                <span class="iconfont icon-guanbi" @click="close"></span> 
            </div>

            <div class="modal-nav">
                <span class="tab-indicator" :style="indicatorStyle"></span>
                <button :class="{ active: selectedTab === 'switchClass' }" @click="selectedTab = 'switchClass'">
                    班级选择
                </button>
                <button :class="{ active: selectedTab === 'approvalList' }" @click="selectedTab = 'approvalList'">
                    审批列表
                </button>
            </div>

            <div class="modal-body">
                <div v-if="selectedTab === 'switchClass'" class="class-list">
                    <div
                    v-for="(classItem, index) in classList"
                    :key="index"
                    :class="{ 'selected': selectedClass === classItem.id }"
                    @click="selectClass(classItem)"
                    class="class-item"
                    >
                    <span class="class-info">{{ classItem.name }}</span>
                    <span class="class-id"> ID: {{ classItem.id }}</span>
                    <span v-if="classItem.isLeader" class="badge">班长</span>
                    </div>
                </div>

                <div v-if="selectedTab === 'approvalList'" class="approval-list">
                    <div v-for="(item, index) in approvalList" :key="index" class="approval-item">
                        <div class="ap-section1">
                            <span class="approval-span">{{ item.name }}</span>
                            <span class="approval-info">申请加入</span>
                            <span class="approval-span">{{ item.class }}</span>
                        </div>
                        <div class="ap-section2">
                            <button class="approve-button" @click="approve(item)">同意</button>
                            <button class="reject-button" @click="reject(item)">拒绝</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </transition>

</template>
  
<script>
import { classApi } from '@/api/index'
export default {
    inject: ['reload'],
    data() {
      return {
        userId: this.$store.state.user.id,
        selectedTab: 'switchClass',
        selectedClass: this.$store.state.classId,
        classList: [],
        approvalList: [],
      };
    },
    computed: {
        // 根据选中选项卡的位置动态调整指示线样式
        indicatorStyle() {
            return {
                left: this.selectedTab === 'switchClass' ? '0%' : '50%',
                width: '50%',
            };
        },
    },
    methods: {
        close() {
            this.$emit("closeManage");
            // this.reload();
        },
        selectClass(classItem) {
            this.selectedClass = classItem.id;
            this.$store.commit('getClass', classItem.id);
            // console.log(classItem);
            this.$message({ type: "success", message: "已切换至"+classItem.name });
            this.$emit("switched");
        },
        async approve(item) {
            try {
                let data = {
                    classId: item.classId,
                    classmateId: item.userId,
                    decision: true
                }
                const response = await classApi.approveJoinClass(data);
                if (response.response == "Approval successful.") {
                    this.$message({ type: "success", message: "同意成功！"});
                }
                else {
                    this.$message({ type: "error", message: "同意失败。"});
                }
                this.approvalList = [];
                this.getApproval();
            }
            catch (e) {
                this.$message({ type: "error", message: "同意失败。"});
            }
        },
        async reject(item) {
            try {
                let data = {
                    classId: item.classId,
                    classmateId: item.userId,
                    decision: false
                }
                const response = await classApi.approveJoinClass(data);
                if (response.response == "Approval successful.") {
                    this.$message({ type: "success", message: "拒绝成功！"});
                }
                else {
                    this.$message({ type: "error", message: "拒绝失败。"});
                }
                this.approvalList = [];
                this.getApproval();
            }
            catch (e) {
                this.$message({ type: "error", message: "拒绝失败。"});
            }
        },
        async getClass() {
            try {
                const response = await classApi.getClassList(this.userId); 

                // 直接将 map 结果赋值给 this.classList
                this.classList = response.map(nclass => ({
                    id: nclass.classID,
                    name: nclass.name,
                    isLeader: nclass.monitor === this.userId,
                }));

                // console.log(response); 
            }
            catch (e) {
                console.error(e)
            }
        },
        async getApproval() {
            try {
                const response = await classApi.getApprovalList(this.userId);
                this.approvalList = response.map(approval => ({
                    name: approval.userName,
                    class: approval.className,
                    userId: approval.userID,
                    classId: approval.classID,
                }));
                // console.log(this.approvalList)
            }
            catch (e) {
                console.error(e)
            }
        }
    },
    created() {
        // this.getClass();
    },
    mounted() {
        this.getClass();
        this.getApproval();
    }
  };
</script>
  
<style lang="less" scoped>
.manage {
    &-enter { // 入场动画，这里只变化透明度
        &-from { 
            opacity: 0;
        }
        &-active { 
            transition: all 0.2s ease-out;
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
            transition: all 0.2s ease-in;  // 慢速开始
        }
        &-to {
            opacity: 0;
        }
    }
}
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
  }
  
  .modal {
    background-color: white;
    width: 450px; /* 固定宽度 */
    height: 540px; /* 固定高度 */
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.468);
    display: flex;
    flex-direction: column;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 56px;
    padding: 19px 24px 8px 24px;
    font-size: 18px;
    font-weight: bold;
    // border-bottom: 1px solid #ddd;
    box-sizing: border-box;
  }
  
  .icon-guanbi {
    background: none;
    border: none;
    font-size: 18px;
    color: @gray-1;
    cursor: pointer;
    margin-top: -2px;
  }
  
  .modal-nav {
    display: flex;
    position: relative;
    border-bottom: 1px solid #ddd;
  }
  
  .modal-nav button {
    flex: 1;
    padding: 10px;
    font-size: 16px;
    
    cursor: pointer;
    background: none;
    border: none;
    color: #555;
    // border-bottom: 2px solid transparent;
    transition: color 0.3s;
  }
  
  .modal-nav button.active {
    font-weight: bold;
    color: @primary-color;
    // border-bottom-color: @primary-color;
  }

  .tab-indicator {
    position: absolute;
    bottom: 0;
    height: 2px;
    width: 50%;
    background-color: #007bff;
    transition: left 0.2s ease-in, width 0.2s ease-in-out;
}
  
  .modal-body {
    flex: 1;
    padding: 32px;
    overflow-y: auto; /* 内部滚动 */
  }
  
  .class-list, .approval-list {
    display: flex;
    flex-direction: column;
    gap: 24px;
  }
  
  .class-item {
    display: flex;
    // justify-content: space-between;
    align-items: center;
    padding: 10px;
    cursor: pointer;
    border: 1px solid #ddd;
    border-radius: 8px;
    transition: 0.3s;
    position: relative;
  }
  
  .class-item:hover, .class-item.selected {
    background-color: #71b2fd70;
    border-color: #71b2fd1d;
  }

  .class-info {
    padding-left: 10px;
    font-weight: 400px;
    font-size: 15px;
  }

  .class-id {
    padding-left: 10px;
    font-size: 12px;
    font-weight: 200px;
    font-style: oblique;
  }
  
  .badge {
    background-color: #ff9800;
    color: white;
    padding: 4px 8px;
    font-size: 12px;
    border-radius: 12px;
    position: absolute;
    top: -6px;
    right: -6px;
  }
  
  .approval-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px 16px 10px 16px;
    border: 1px solid #ddd;
    border-radius: 8px;
  }

  .approval-span {
    font-weight: 600px;
    font-size: 15px;
  }
  
  .approval-info {
    padding: 0px 8px;
    font-size: 13px;
    font-weight: 200px;
   }
  
  .approve-button, .reject-button {
    padding: 6px 12px;
    font-size: 14px;
    border: none;
    cursor: pointer;
    border-radius: 4px;
    margin-left: 4px;
  }
  
  .approve-button {
    background-color: #4caf4fe1;
    color: white;
    transition: 0.3s;
  }
  
  .reject-button {
    margin-left: 10px;
    background-color: #f44336e4;
    color: white;
    transition: 0.3s;
  }

  .approve-button:hover {
    background-color: #4caf50;
  }

  .reject-button:hover {
    background-color: #f44336;
  }
  </style>