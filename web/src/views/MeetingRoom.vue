<template>
  <p class="ctitle">小聚厅</p>
  <p class="slogan">来聚</p>
  <div class="meeting-room">
    <el-drawer
        v-model="drawer"
        title="小聚图片"
        :direction="'btt'"
        size="70%"
    >
      <el-row :gutter="20">
        <el-col :span="6" v-for="url in urls" :key="url">
          <el-image style="width: 22vw; height: 22vw; margin-top: 20px" :src="url" lazy :fit="'cover'"/>
        </el-col>
      </el-row>
    </el-drawer>
    <el-dialog v-model="dialogTableVisible" :before-close="closeCreateModal" title="上传图片" width="400" style="z-index: 9999; display: block;">
      <el-upload
          ref="uploadRef"
          class="avatar-uploader"
          :drag="true"
          :auto-upload="false"
          :show-file-list="false"
          :on-change="onUploadFile"
      >
        <img v-if="newMeeting.imgUrl" :src="newMeeting.imgUrl" class="avatar" alt=""/>
        <el-empty v-else :image-size="50"/>
      </el-upload>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogTableVisible = false, closeCreateModal()">取消</el-button>
          <el-button type="primary" @click="addPicture">
            确认
          </el-button>
        </div>
      </template>
    </el-dialog>
    <!-- 左侧按钮区 -->
    <div class="left-panel">
      <div class="filter-buttons">
        <div class="filter-btn" :class="{active: currentFilter === 'all'}" @click="setFilter('all')">
          <span class="iconfont icon-quanbu"></span>
          <p>全部</p>
        </div>
        <div class="filter-btn" :class="{active: currentFilter === 'joined'}" @click="setFilter('joined')">
          <span class="iconfont icon-jiaru"></span>
          <p>我加入的</p>
        </div>
        <div class="filter-btn" :class="{active: currentFilter === 'created'}" @click="setFilter('created')">
          <span class="iconfont icon-faqi"></span>
          <p>我发起的</p>
        </div>
      </div>
    </div>
    <!-- 中间聚会列表区 -->
    <div class="meeting-list">
      <div class="list-container" ref="listContainer">
        <div class="meeting-card"
             v-for="(meeting) in filteredMeetings"
             :key="meeting.id"
             :data-id="meeting.id"
             @click="showParticipants(meeting)"
        >
          <div class="card-header">
            <p class="time">{{ formatDate(meeting.time) }}</p>
            <p class="location">{{ meeting.location }}</p>
          </div>
          <div class="card-body">
            <p class="description">{{ meeting.description }}</p>
            <div class="participants">
              <span class="count">{{ meeting.participantsCount }}人参与</span>
            </div>
          </div>
          <div class="card-footer">
            <div class="status" :class="meeting.status">
              {{ getStatusText(meeting.status) }}
            </div>
            <div class="action-buttons">
              <ct-buttons-vue
                  size="small"
                  nom="primary"
                  @click.stop="addPicturePre(meeting.id)"
              >
                新增图片
              </ct-buttons-vue>
            </div>
            <div class="action-buttons">
              <ct-buttons-vue
                  size="small"
                  nom="primary"
                  @click.stop="showPicture(meeting.id)"
              >
                查看图片
              </ct-buttons-vue>
            </div>
            <div class="action-buttons">
              <ct-buttons-vue
                  v-if="meeting.status === 'not-joined'"
                  size="small"
                  nom="primary"
                  @click.stop="joinMeeting(meeting)"
              >
                加入聚会
              </ct-buttons-vue>
              <ct-buttons-vue
                  v-if="meeting.status === 'joined'"
                  size="small"
                  nom="warning"
                  @click.stop="exitMeeting(meeting)"
              >
                退出聚会
              </ct-buttons-vue>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- 右侧创建区 -->
    <div class="right-panel">
      <div class="create-card" @click="createNewMeeting">
        <div class="plus-icon">
          <span class="iconfont icon-jia"></span>
        </div>
        <p class="create-text">创建新的聚会</p>
      </div>
    </div>
    <!-- 添加参与者信息弹窗 -->
    <div class="participants-modal" v-if="showModal" @click.self="closeModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>参与者信息</h3>
          <span class="iconfont icon-guanbi" @click="closeModal"></span>
        </div>
        <div class="modal-body">
          <div class="meeting-info">
            <p class="time">{{ formatDate(selectedMeeting.time) }}</p>

            <p class="location">{{ selectedMeeting.location }}</p>
            <p class="description">{{ selectedMeeting.description }}</p>
            <!-- 添加操作按钮 -->
            <div class="meeting-actions" v-if="selectedMeeting">
              <!-- 如果是发起人显示修改和删除按钮 -->
              <template v-if="selectedMeeting.initiator_id === currentUserId">
                <ct-buttons-vue size="base" nom="secondary" @click="openEditModal">修改</ct-buttons-vue>
                <ct-buttons-vue size="base" nom="secondary" @click="confirmDelete">删除</ct-buttons-vue>
              </template>
              <!-- 如果参与者显示退出按钮 -->
              <template v-else-if="selectedMeeting.status === 'joined'">
                <ct-buttons-vue size="base" nom="secondary" @click="confirmExit">退出</ct-buttons-vue>
              </template>
            </div>
          </div>
          <div class="participants-list">
            <div class="participant-item" v-for="participant in selectedMeeting.participants" :key="participant.id">
              <div class="participant-avatar" :style="{ backgroundImage: `url(${participant.avatar})` }"></div>
              <div class="participant-info">
                <p class="name">{{ participant.name }}</p>
                <p class="status">{{ participant.status }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- 创建小聚的弹窗 -->
    <div class="create-modal" v-if="showCreateModal" @click.self="closeCreateModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>创建新的聚会</h3>
          <span class="iconfont icon-guanbi" @click="closeCreateModal"></span>
        </div>
        <div class="modal-body">
          <div class="form-item">
            <label>时间</label>
            <input type="datetime-local" v-model="newMeeting.datetime">
          </div>
          <div class="form-item">
            <label>地点</label>
            <input type="text" v-model="newMeeting.address" placeholder="请输入聚会地点">
          </div>
          <div class="form-item">
            <label>描述</label>
            <textarea v-model="newMeeting.description" placeholder="请输入聚会描述"></textarea>
          </div>
          <!--
          <div class="form-item">
            <label>封面图片</label>
            <el-upload
                ref="uploadRef"
                class="avatar-uploader"
                :drag="true"
                :auto-upload="false"
                :show-file-list="false"
                :on-change="onUploadFile"
            >
              <img v-if="newMeeting.imgUrl" :src="newMeeting.imgUrl" class="avatar" alt=""/>
              <el-empty v-else :image-size="50"/>
            </el-upload>
          </div>
          -->
        </div>
        <div class="modal-footer">
          <ct-buttons-vue size="base" nom="secondary" @click="closeCreateModal">取消</ct-buttons-vue>
          <ct-buttons-vue size="base" nom="primary" @click="submitNewMeeting">创建</ct-buttons-vue>
        </div>
      </div>
    </div>
    <!-- 添加修改小聚的弹窗 -->
    <div class="edit-modal" v-if="showEditModal" @click.self="closeEditModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>修改小聚</h3>
          <span class="iconfont icon-guanbi" @click="closeEditModal"></span>
        </div>
        <div class="modal-body">
          <div class="form-item">
            <label>时间</label>
            <input type="datetime-local" v-model="editMeeting.datetime">
          </div>
          <div class="form-item">
            <label>地点</label>
            <input type="text" v-model="editMeeting.address" placeholder="请输入聚会地点">
          </div>
          <div class="form-item">
            <label>描述</label>
            <textarea v-model="editMeeting.description" placeholder="请输入聚会描述"></textarea>
          </div>
        </div>
        <div class="modal-footer">
          <ct-buttons-vue size="base" nom="secondary" @click="closeEditModal">取消</ct-buttons-vue>
          <ct-buttons-vue size="base" nom="primary" @click="submitEdit">确定</ct-buttons-vue>
        </div>
      </div>
    </div>
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
          <!--加入班级-->
          <join-class-vue v-show="isJoining==1" @closeJoin="closeJoin"></join-class-vue>

<!--新建班级-->
<new-class-vue v-show="isCreating==1" @closeCreate="closeCreate"></new-class-vue>

<!--班级管理-->
<manage-class-vue v-show="isManaging==1" @closeManage="closeManage" @switched="classSwitched"></manage-class-vue>
</template>

<script>
import {gatheringApi, personalApi, commentApi} from '@/api/index'
import CTButtons from '@/components/CTButtons.vue'
import OSS from 'ali-oss';
import {v4 as uuidv4} from 'uuid';
import joinClassVue from '@/components/JoinClass.vue'
import newClassVue from '@/components/NewClass.vue'
import manageClassVue from '@/components/ManageClass.vue'

import { insertWallApi } from "@/api/index"

import { dataOne } from '@/utils/methods'

import { format } from 'date-fns';

export default {
  name: 'MeetingRoom',
  inject: ['reload'],
  components: {
    joinClassVue,
        newClassVue,
        manageClassVue,
    'ct-buttons-vue': CTButtons  // 注册组件
  },
  data() {
    return {
      addBottom: 50, // add按钮的bottom距离变量
      dataOne,
      isJoining: 0,
            isCreating: 0,
            isManaging: 0,
      currentFilter: 'all',
      meetings: [],
      isDragging: false,
      startX: 0,
      scrollLeft: 0,
      showModal: false,
      selectedMeeting: null,
      showCreateModal: false,
      drawer: false,
      dialogTableVisible: false,
      files: '',
      newMeeting: {
        datetime: format(new Date(), "yyyy-MM-dd'T'HH:mm"),
        address: '',
        description: '',
        imgUrl: ""
      },
      tempId: "",
      url: 'https://inews.gtimg.com/om_bt/O6SG7dHjdG0kWNyWz6WPo2_3v6A6eAC9ThTazwlKPO1qMAA/641',
      showEditModal: false,
      editMeeting: {
        datetime: format(new Date(), "yyyy-MM-dd'T'HH:mm"),
        address: '',
        description: ''
      },
      urls: [],
      // 添加静态用户数据
      mockUsers: {
        '1': {
          name: '我',
          avatar: 'https://placekitten.com/40/40',
          status: '发起人'
        },
        '2': {
          name: '李四',
          avatar: 'https://placekitten.com/41/41',
          status: '已加入'
        },
        '5': {
          name: '赵六',
          avatar: 'https://placekitten.com/42/42',
          status: '已加入'
        },
        '13': {
          name: '王五',
          avatar: 'https://placekitten.com/43/43',
          status: '发起人'
        }
      },
      // 添加静态照片数据
      mockPictures: [
        {
          id: 1,
          url: 'https://placekitten.com/300/200',
          description: '聚会照片1'
        },
        {
          id: 2,
          url: 'https://placekitten.com/301/200',
          description: '聚会照片2'
        }
      ],
      currentUserId: String(this.$store.state.user.id),// '1',  // 修改这里，从 '13' 改为 '1'
    }
  },
  async created() {
    await this.fetchMeetings()
  },
  computed: {
    filteredMeetings() {
      let filtered = [];
      switch (this.currentFilter) {
        case 'joined':
          filtered = this.meetings.filter(m => {
            const isJoined = m.join_users && m.join_users.includes(this.currentUserId);
            return isJoined;
          });
          break;
        case 'created':
          filtered = this.meetings.filter(m => {
            const isCreated = m.initiator_id === this.currentUserId;
            return isCreated;
          });
          break;
        default:
          filtered = this.meetings;
          break;
      }

      // 根据时间从新到旧排序
      filtered.sort((a, b) => new Date(b.time) - new Date(a.time));
      return filtered;
    }
  },
  watch: {
    currentFilter() {
      this.$nextTick(() => {
        this.filteredMeetings.forEach(async (item) => {
          this.applyBackgroundImage(item.id);
        });
      });
    }
  },
  mounted() {
    // 添加鼠标滚轮事件监听
    const listContainer = this.$refs.listContainer
    if (listContainer) {
      listContainer.addEventListener('wheel', this.handleWheel, {passive: false})
    }
  },
  beforeUnmount() {
    // 移除事件监听
    const listContainer = this.$refs.listContainer
    if (listContainer) {
      listContainer.removeEventListener('wheel', this.handleWheel)
    }
  },
  methods: {
    classSwitched() {
      this.reload();
    },
    async setFilter(filter) {
      this.currentFilter = filter
    },
    formatDate(dateTime) {
      const d = new Date(dateTime);
      const year = d.getFullYear();
      const month = (d.getMonth() + 1).toString().padStart(2, '0');
      const day = d.getDate().toString().padStart(2, '0');
      const hours = d.getHours().toString().padStart(2, '0');
      const minutes = d.getMinutes().toString().padStart(2, '0');
      return `${year}.${month}.${day} ${hours}:${minutes}`; // 格式为 "YYYY.MM.DD HH:MM"
    },
    getStatusText(status) {
      const statusMap = {
        'not-joined': '未加入',
        'joined': '已加入',
        'created': '我发起的',
        'ended': '已结束'
      }
      return statusMap[status]
    },
    getStatusClass(status) {
      return {
        'not-joined': status === 'not-joined',
        'joined': status === 'joined',
        'created': status === 'created',
        'ended': status === 'ended'
      }
    },
    async applyBackgroundImage(id) {
      // 获取小聚图片，随机抽取一张作为封面
      const params = {
        gathering_id: id,
        user_id: this.$store.state.user.id,
      }
      const axiosResponse = await gatheringApi.getGatheringPictures(params);

      // 收集图片 URL
      const images = axiosResponse.map(item => {
        return item.image
      });
      console.log("id:" + id + " 图片有：" + images);

      // 使用 data-id 选择器获取对应的 DOM 元素
      const element = document.querySelector(`.meeting-card[data-id="${id}"]`);

      // 随机选择一张图片作为背景
      let number = Math.floor(Math.random() * images.length);
      if (images && images.length > 0 && images[number]) {
        try {
        element.style.backgroundImage = `url("${images[number]}")`;}
        catch (e) {
          console.error("id:" + id + " 获取到的图片URL无效"+e);
        }
        // element.style.backgroundImage = `url("${images}")`;
      } else {
        // element.style.backgroundImage = `url("http://markdown-wky.oss-cn-wuhan-lr.aliyuncs.com/454dd9ba-a0cf-4e15-9b8f-11dafe8e5515")`;
        console.error("id:" + id + " 获取到的图片URL无效");
      }
    },
    async fetchMeetings() {
      try {
        const params = {
          class_id: this.$store.state.classId,
        }
        const response = await gatheringApi.getGatheringList(params)

        if (Array.isArray(response)) {
          console.log('Raw API response:', response)
          response.forEach(async (item, index) => {
            console.log("获取到" + item.gaID, index)
            await this.applyBackgroundImage(item.gaID, index)
          })
          this.meetings = response.map(meeting => {
            console.log('Complete meeting data:', JSON.stringify(meeting, null, 2))


            // 适配后端字名
            const meetingData = {
              id: meeting.gaID,
              time: meeting.date,
              location: meeting.position,
              description: meeting.discription,
              initiator_id: String(meeting.proposer),
              // 修改这里，使用 Participant 字段
              join_users: meeting.participant ?
                  meeting.participant.map(id => String(id)) : [],
              participantsCount: meeting.participant ?
                  meeting.participant.length : 0
            }

            // 打印处理后的数据
            console.log('Processed meeting data:', {
              id: meetingData.id,
              join_users: meetingData.join_users,
              raw_participants: meeting.participant,
              currentUserId: this.currentUserId,
              initiator_id: meetingData.initiator_id
            })

            // 获取状态
            const status = this.getStatus({
              datetime: meetingData.time,
              initiator_id: meetingData.initiator_id,
              join_users: meetingData.join_users
            })

            return {
              ...meetingData,
              status
            }
          })
          this.$nextTick(async () => {
            await Promise.all(this.filteredMeetings.map(meeting => this.applyBackgroundImage(meeting.id)));
          });
          // 打印最终的会议列表
          console.log('Final meetings list:', this.meetings.map(m => ({
            id: m.id,
            initiator_id: m.initiator_id,
            join_users: m.join_users,
            status: m.status
          })))
        }
      } catch (error) {
        console.error('获取小聚列表失败:', error)
        this.meetings = []
      }
    },
    async onUploadFile(file) {
      this.files = file.raw;
      const reader = new FileReader();
      reader.readAsDataURL(file.raw);
      reader.onload = () => {
        this.newMeeting.imgUrl = reader.result;
      };
    },
    getStatus(meeting) {
      const now = new Date()
      const meetingTime = new Date(meeting.datetime)

      console.log('Checking status for meeting:', {
        initiator_id: meeting.initiator_id,
        currentUserId: this.currentUserId,
        join_users: meeting.join_users,
        meetingTime: meetingTime,
        now: now
      })

      // 如果是发起人
      if (String(meeting.initiator_id) === this.currentUserId) {
        return 'created'
      }
      // 如果已经加入（包括 '0' 的情况）
      if (meeting.join_users && meeting.join_users.includes(this.currentUserId)) {
        return 'joined'
      }

      // 如果已经结束(时间过期)
      if (!isNaN(meetingTime.getTime()) && meetingTime < now) {
        return 'ended'
      }
      return 'not-joined'
    },
    async createNewMeeting() {
      this.showCreateModal = true
      // 设置默认时间为当前时间
      // const now = new Date()
      // this.newMeeting.datetime = now
      console.log(this.newMeeting.datetime)
    },
    closeCreateModal() {
      this.dialogTableVisible = false
      this.showCreateModal = false
      this.newMeeting = {
        datetime: format(new Date(), "yyyy-MM-dd'T'HH:mm"),
        address: '',
        description: '',
        imgUrl: ""
      }
      this.files = ""
    },
    async submitNewMeeting() {
      try {
        if (!this.newMeeting.datetime || !this.newMeeting.address || !this.newMeeting.description) {
          alert('请填写完整信息')
          return
        }
        console.log(this.newMeeting.datetime)
        const params = {
          class_id: this.$store.state.classId,
          user_id: this.currentUserId,
          address: this.newMeeting.address.trim(),
          description: this.newMeeting.description.trim(),
          datetime: this.newMeeting.datetime.replace('T', ' ')+':00',
        }

        console.log(params)
        const result = await gatheringApi.createGathering(params)
        if (result) {
          this.closeCreateModal()
          await this.fetchMeetings()
          alert('创建成功！')
          this.reload();
        }
      } catch (error) {
        console.error('创建小聚失败:', error)
        alert('创建失败：' + error.message)
      }
    },
    startDrag(e) {
      this.isDragging = true
      this.startX = e.pageX - this.$refs.scrollThumb.offsetLeft
      document.addEventListener('mousemove', this.onDrag)
      document.addEventListener('mouseup', this.stopDrag)
    },
    onDrag(e) {
      if (!this.isDragging) return
      e.preventDefault()
      const container = this.$refs.listContainer
      const x = e.pageX - this.startX
      const scrollBarWidth = this.$refs.scrollThumb.parentNode.clientWidth
      const thumbWidth = this.$refs.scrollThumb.clientWidth
      const maxX = scrollBarWidth - thumbWidth
      const scrollX = Math.max(0, Math.min(x, maxX))
      const scrollRatio = scrollX / maxX
      container.scrollLeft = scrollRatio * (container.scrollWidth - container.clientWidth)
    },
    stopDrag() {
      this.isDragging = false
      document.removeEventListener('mousemove', this.onDrag)
      document.removeEventListener('mouseup', this.stopDrag)
    },
    async showParticipants(meeting) {
      try {
        // 动态获取用户头像和其他信息
        const uniqueUserIds = Array.from(new Set([meeting.initiator_id, ...meeting.join_users]));
        const participants = await Promise.all(
          uniqueUserIds.map(async (userId) => {
            try {
              const response = await personalApi.getPersonalInfo(userId); // 获取用户详细信息
              console.log(`用户 ${userId} 的 API 响应:`, response); // 打印 API 响应

              // 直接使用 response，因为返回的数据不是包裹在 data 中的
              return {
                id: userId,
                name: response.name || `用户${userId}`, // 用户名
                avatar: response.headPicture || '默认头像URL', // 头像 URL
                status: userId === meeting.initiator_id ? '发起人' : '参与者' // 状态
              };
            } catch (error) {
              console.error(`获取用户 ${userId} 信息失败:`, error);
              return null; // 获取失败返回 null
            }
          })
        );

        // 过滤掉获取信息失败的用户
        const filteredParticipants = participants.filter((participant) => participant !== null);

        console.log('过滤后的参与者列表:', filteredParticipants);

        this.selectedMeeting = {
          ...meeting,
          participants: filteredParticipants
        };
        this.showModal = true;
      } catch (error) {
        console.error('获取小聚参与者信息失败:', error);
      }
    },
    closeModal() {
      this.showModal = false
      this.selectedMeeting = null
    },
    async deleteGathering(meetingId) {
      try {
        const params = {
          gathering_id: meetingId,
          user_id: this.$store.state.user.id, // 需要从vuex获取
        }
        await gatheringApi.deleteGathering(params)
        // 删除成功后刷新列表
        await this.fetchMeetings()
        this.closeModal()
      } catch (error) {
        console.error('删除小聚失败:', error)
      }
    },
    async addPicture() {
      if (!this.newMeeting.imgUrl) {
        alert('请上传图片')
        return
      }
      this.dialogTableVisible = false

      const client = new OSS({
        region: '',
        accessKeyId: '',
        accessKeySecret: '',
        bucket: ''
      });

      try {
        const uuid = uuidv4();
        const directoryPath = 'lcy'; // 指定目录路径
        const objectKey = `${directoryPath}${uuid}`; // 将路径与文件名拼接

        const r1 = await client.put(objectKey, this.files); // 使用完整路径上传
        console.log('put success: %j', r1.url);
        const r2 = await client.get(objectKey);
        console.log('get success: %j', r2);

        const targetMeeting = this.meetings.find(meeting => meeting.id === this.tempId);

        const params = {
          user_id: this.currentUserId,
          class_id: this.$store.state.classId,
          picture_url: r1.url,
          address: targetMeeting.location,
          description: targetMeeting.description,
          gathering_id: this.tempId
        }

        let data = {
            type: 1, // 留言/照片
            message: params.description, 
            name: dataOne(String(targetMeeting.time))+'的小聚',
            userId: this.currentUserId,
            moment: new Date(),
            label: 7+this.tempId,
            color: 5,
            imgurl: r1.url,  // 图片会在uploadPhoto(data)赋值给它
            classId: this.$store.state.classId,
        };

        insertWallApi(data).then((result) => {  // 后端提交
          console.log(result)
            });

        await gatheringApi.uploadGatheringPictures(params)

        // 更新留言
        let data_async = {
            classId: data.classId,
            comment: data.message,
            userId: -1,
        }
        const response_async = await commentApi.postComment(data_async);
        console.log(response_async);
        this.reload();
      } catch (e) {
        console.error('error: %j', e);
      }
      this.newMeeting.imgUrl = ""
    },
    async showPicture(id) {
      const params = {
        // gathering_id: this.selectedMeeting.id,
        gathering_id: id,
        user_id: this.$store.state.user.id,
      };
      try {
        const axiosResponse = await gatheringApi.getGatheringPictures(params);
        this.drawer = true;
        // 新增遍历数组并收集图片 URL 的逻辑
        this.urls = axiosResponse.map(item => {
          console.log("item.image:",item.image)
          return item.image
        });
      } catch (error) {
        console.log("error" + error)
        alert('获取小聚图片失败');
      }
    },
    // 关闭修改弹窗
    closeEditModal() {
      this.showEditModal = false
      this.editMeeting = {
        datetime: '',
        address: '',
        description: ''
      }
    },
    // 添加新增图片方法前置处理
    addPicturePre(id){
      this.dialogTableVisible = true
      this.tempId = id
    },
    // 提交修改
    async submitEdit() {
      try {
        if (!this.editMeeting.datetime || !this.editMeeting.address) {
          alert('请填写完整信息')
          return
        }
        const params = {
          gathering_id: this.selectedMeeting.id,
          datetime: this.editMeeting.datetime.replace('T', ' ')+':00',
          address: this.editMeeting.address.trim(),
          description: this.editMeeting.description.trim(),
          user_id: this.$store.state.user.id,
        }
        console.log(params)
        console.log('Updating meeting with params:', params)
        await gatheringApi.updateGathering(params)
        // 刷新数据
        await this.fetchMeetings()
        this.closeEditModal()
        this.closeModal()
        alert('修改成功！')
      } catch (error) {
        console.error('修改小聚失败:', error)
        alert('修改失败：' + error.message)
      }
    },

    // 确出
    async confirmExit() {
      if (confirm('确要退出这个小聚吗？')) {
        try {
          const params = {
            gathering_id: this.selectedMeeting.id,
            user_id: this.$store.state.user.id,
          }
          await gatheringApi.exitGathering(params)
          // 退出成功后刷新列表
          await this.fetchMeetings()
          this.closeModal()
        } catch (error) {
          console.error('退出小聚失败:', error)
        }
      }
    },
    // 确认删除
    async confirmDelete() {
      if (confirm('确定要删除这个小聚吗？')) {
        await this.deleteGathering(this.selectedMeeting.id)
      }
    },
    // 处理鼠标滚轮事件
    handleWheel(e) {
      e.preventDefault() // 阻止默认的垂直滚动
      const container = this.$refs.listContainer
      if (container) {
        // 如果按住 Shift 键或者是触控板的横向滚动，使用 deltaX
        const delta = Math.abs(e.deltaX) > Math.abs(e.deltaY) ? e.deltaX : e.deltaY
        container.scrollLeft += delta
      }
    },
    // 添加加入聚会方法
    async joinMeeting(meeting) {
      try {
        const params = {
          gathering_id: meeting.id,
          user_id: this.$store.state.user.id
        }
        console.log('Joining meeting with params:', params)
        await gatheringApi.joinGathering(params)

        // 刷新数据
        await this.fetchMeetings()
        alert('加入成功！')
      } catch (error) {
        console.error('加入聚会失败:', error)
        alert('加入失败：' + error.message)
      }
    },
    // 修改退出聚会方法
    async exitMeeting(meeting) {
      try {
        const params = {
          gathering_id: meeting.id
        }
        console.log('Exiting meeting with params:', params)
        await gatheringApi.exitGathering(params)
        // 刷新数据
        await this.fetchMeetings()
        alert('退出成功！')
      } catch (error) {
        console.error('退出聚会失败:', error)
        alert('退出失败：' + error.message)
      }
    },
    // 添加显示修改弹窗的方法
    openEditModal() {
      // 初始化编辑数据
      this.editMeeting = {
        datetime: this.selectedMeeting.time,
        address: this.selectedMeeting.location,
        description: this.selectedMeeting.description || ''
      }
      this.showEditModal = true  // 使用新的变量名
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
.ctitle {
    position: absolute;
    top: 0; /* 放置在页面最上方 */
    left: 50%; /* 水平居中 */
    transform: translateX(-50%); /* 水平居中 */
    padding-top: 100px;
    padding-bottom: @padding-8;
    font-size: 56px;
    color: @gray-0;
    text-align: center;
    font-weight: 600;
}
.slogan {
    position: absolute;
    top: 0; /* 放置在页面最上方 */
    left: 50%; /* 水平居中 */
    transform: translateX(-50%); /* 水平居中 */
    padding-top: 190px;
    color: @gray-1;
    text-align: center;
}
.meeting-room {
  min-height: calc(100vh - 252px);
  padding-top: 232px;
  display: flex;
  width: 100%;

  .left-panel {
    width: 200px;
    padding: 20px;
    border-right: 1px solid rgba(0, 0, 0, 0.1);

    .filter-buttons {
      .filter-btn {
        display: flex;
        align-items: center;
        padding: 12px;
        margin-bottom: 12px;
        border-radius: 8px;
        cursor: pointer;
        transition: @tr;

        &:hover {
          background: rgba(59, 115, 240, 0.1);
        }

        &.active {
          background: @primary-color;
          color: white;
        }

        .iconfont {
          font-size: 20px;
          margin-right: 8px;
        }

        p {
          font-size: @size-14;
        }
      }
    }
  }


  .meeting-list {
    flex: 0 0 996px; /* 固定宽度，不允许拉伸或缩小 */
    padding: 20px;
    position: relative;
    width: 996px;

    .list-container {
      display: flex; // 使用 flex 布局
      overflow-x: scroll; // 添加横向滚动条
      padding-bottom: 20px;

      // 自定义滚动条样式
      &::-webkit-scrollbar {
        height: 8px; // 滚动条高度
      }

      &::-webkit-scrollbar-track {
        background: rgba(0, 0, 0, 0.1); // 滚动条轨道
        border-radius: 4px;
      }

      &::-webkit-scrollbar-thumb {
        background: @primary-color; // 滚动条滑块
        border-radius: 4px;

        &:hover {
          background: darken(@primary-color, 10%); // 悬停时变暗
        }
      }

      .meeting-card {
        flex: 0 0 300px; // 固定度，不缩放
        height: 55vh; // 添加固定高度
        margin-right: 20px;
        background-size: 100% 80%;
        background-position: top;
        background-repeat: no-repeat;
        border-radius: 12px;
        box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
        padding: 24px;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        cursor: pointer;
        transition: @tr; // 添加过渡效果

        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
        }

        .card-header {
          margin-bottom: 12px;

          .time {
            font-size: 24px; /* 字体大小 */
            font-weight: 1000; /* 字体粗细 */
            font-family: 'wordF'; /* 等宽字体 */
            text-shadow: 0px 4px 8px rgba(0, 0, 0, 0.051); /* 柔和的阴影 */
            margin-bottom: 4px;

            /* 采用梦幻渐变色 */
            background: linear-gradient(90deg, #A390E4, #C3ADE1, #91D8D2, #F3C5CD, #FFD1DC); /* 梦幻渐变色 */
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
          }


          .location {
            font-size: @size-14;
            color: @gray-1;
          }
        }

        .card-body {
          flex: 1;
          display: flex;
          flex-direction: column;
          justify-content: space-between;

          .description {
            font-size: @size-14;
            color: @gray-1;
            margin-bottom: 12px;
            line-height: 1.5;
          }

          .participants {
            font-size: @size-12;
            color: @gray-2;
          }
        }

        .card-footer {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding-top: 16px;
          border-top: 1px solid rgba(0, 0, 0, 0.1);
          font-size: @size-12;

          .action-buttons {
            margin-left: 5px;
            display: flex;
            gap: 8px;

            .ct-buttons-vue {
              padding: 4px 12px;
              font-size: @size-12;
              border-radius: 12px;

              &:hover {
                opacity: 0.9;
              }
            }
          }
        }

        .ct-buttons {
          padding: 0;
        }
      }
    }

    // 移除自定义滚动条组件
    .scroll-bar {
      display: none;
    }
  }

  .avatar {
    max-width: 100%; /* 限制图片宽度 */
    max-height: 15vw; /* 限制图片高度 */
    margin: 0 auto;
  }

  .right-panel {
    width: 300px;
    padding: 20px;
    border-left: 1px solid rgba(0, 0, 0, 0.1);

    .create-card {
      height: 380px;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
      padding: 30px;
      text-align: center;
      cursor: pointer;
      transition: @tr;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;

      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
      }

      .plus-icon {
        width: 100px;
        height: 100px;
        background: rgba(59, 115, 240, 0.1);
        border-radius: 50px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin: 0 auto 32px;

        .iconfont {
          font-size: 50px;
          color: @primary-color;
        }
      }

      .create-text {
        font-size: @size-16;
        color: @gray-1;
      }
    }
  }
}

// 添加弹窗样式
.participants-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;

  .modal-content {
    width: 480px;
    max-height: 80vh;
    background: white;
    border-radius: 12px;
    overflow: hidden;

    .modal-header {
      padding: 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-bottom: 1px solid rgba(0, 0, 0, 0.1);

      h3 {
        font-size: @size-16;
        color: @gray-0;
        margin: 0;
      }

      .icon-guanbi {
        cursor: pointer;
        color: @gray-2;
        font-size: 20px;

        &:hover {
          color: @gray-1;
        }
      }
    }

    .modal-body {
      padding: 20px;
      max-height: 70vh; /* 设置最大高度 */
      overflow-y: auto;

      .meeting-info {
        margin-bottom: 24px;
        padding-bottom: 16px;
        border-bottom: 1px solid rgba(0, 0, 0, 0.1);

        .time {
          font-size: @size-16;
          color: @gray-0;
          font-weight: 500;
          margin-bottom: 8px;
        }

        .location, .description {
          font-size: @size-14;
          color: @gray-1;
          margin-bottom: 4px;
        }
      }

      .participants-list {
        .participant-item {
          display: flex;
          align-items: center;
          padding: 12px 0;

          .participant-avatar {
            width: 40px;
            height: 40px;
            border-radius: 20px;
            background-size: cover;
            background-position: center;
            margin-right: 12px;
          }

          .participant-info {
            .name {
              font-size: @size-14;
              color: @gray-0;
              margin-bottom: 4px;
            }

            .status {
              font-size: @size-12;
              color: @gray-2;
            }
          }
        }
      }
    }
  }
}

// 创建小聚的弹窗样式
.create-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  overflow: scroll;
  height: 100vh;

  .modal-content {
    margin-top: 10vh;
    width: 480px;
    background: white;
    border-radius: 12px;
    overflow: hidden;

    .modal-header {
      padding: 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-bottom: 1px solid rgba(0, 0, 0, 0.1);

      h3 {
        font-size: @size-16;
        color: @gray-0;
        margin: 0;
      }

      .icon-guanbi {
        cursor: pointer;
        color: @gray-2;
        font-size: 20px;

        &:hover {
          color: @gray-1;
        }
      }
    }

    .avatar-uploader {
      //width: 300px;
    }

    .avatar {
      max-width: 100%; /* 限制图片宽度 */
      max-height: 15vw; /* 限制图片高度 */
      margin: 0 auto;
    }

    .modal-body {
      padding: 20px;

      .form-item {
        margin-bottom: 20px;

        label {
          display: block;
          color: @gray-0;
          margin-bottom: 8px;
        }

        input, textarea {
          width: 100%;
          padding: 8px 12px;
          border: 1px solid @gray-2;
          border-radius: 4px;
          box-sizing: border-box;

          &:focus {
            border-color: @primary-color;
          }
        }

        textarea {
          height: 100px;
          resize: none;
        }
      }
    }

    .modal-footer {
      padding: 20px;
      border-top: 1px solid rgba(0, 0, 0, 0.1);
      display: flex;
      justify-content: flex-end;
      gap: 12px;
    }
  }
}

// 添加修改小聚的弹窗样式
.edit-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;

  .modal-content {
    width: 480px;
    background: white;
    border-radius: 12px;
    overflow: hidden;

    .modal-header {
      padding: 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-bottom: 1px solid rgba(0, 0, 0, 0.1);

      h3 {
        font-size: @size-16;
        color: @gray-0;
        margin: 0;
      }

      .icon-guanbi {
        cursor: pointer;
        color: @gray-2;
        font-size: 20px;

        &:hover {
          color: @gray-1;
        }
      }
    }

    .modal-body {
      padding: 20px;

      .form-item {
        margin-bottom: 20px;

        label {
          display: block;
          color: @gray-0;
          margin-bottom: 8px;
        }

        input, textarea {
          width: 100%;
          padding: 8px 12px;
          border: 1px solid @gray-2;
          border-radius: 4px;
          box-sizing: border-box;

          &:focus {
            border-color: @primary-color;
          }
        }

        textarea {
          height: 100px;
          resize: none;
        }
      }
    }

    .modal-footer {
      padding: 20px;
      border-top: 1px solid rgba(0, 0, 0, 0.1);
      display: flex;
      justify-content: flex-end;
      gap: 12px;
    }
  }
}

.meeting-actions {
  margin-top: 16px;
  display: flex;
  gap: 12px;
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
</style>
