<template>
    <p class="ctitle">蹭饭图</p>
    <p class="slogan">记录你我的同窗行迹。</p>
    <div class="map-view">
        <!-- 简化侧边栏 -->
        <div class="sidebar">
            <div class="search-box">
                <input 
                    type="text" 
                    v-model="searchText"
                    placeholder="搜索省份/城市..." 
                    class="search-input"
                    @input="handleSearch"
                />
                <span class="iconfont icon-sousuo"></span>
            </div>
            <!-- 添加热门城市列表 -->
            <div class="hot-cities">
                <p class="title">热门城市</p>
                <div class="city-list">
                    <div 
                        v-for="city in hotCities" 
                        :key="city.name"
                        class="city-item"
                        :class="{active: currentCity === city.name}"
                        @click="selectCity(city)"
                    >
                        <p class="city-name">{{city.name}}</p>
                        <p class="city-count">{{city.classmateCount}}位同学</p>
                    </div>
                </div>
            </div>
        </div>
        <div class="main-content">
            <div id="container" class="map-container"></div>
            <!-- 信息窗体 - 同学信息 -->
            <div class="info-window" v-show="showInfo && infoType === 'classmate'" :style="infoPosition">
                <div class="info-content">
                    <div class="info-header">
                        <p class="name">{{currentInfo.name}}</p>
                        <span class="iconfont icon-guanbi" @click="closeInfo"></span>
                    </div>
                    <div class="info-body">
                        <p class="location">当前位置：{{currentInfo.address}}</p>
                        <p class="last-login">最后登录：{{currentInfo.lastLogin}}</p>
                    </div>
                </div>
            </div>
            <!-- 信息窗体 - 聚会信息 -->
            <div class="info-window" v-show="showInfo && infoType === 'meeting'" :style="infoPosition">
                <div class="info-content">
                    <div class="info-header">
                        <p class="time">{{dataTwo(currentInfo.time) || ''}}</p>
                        <span class="iconfont icon-guanbi" @click="closeInfo"></span>
                    </div>
                    <div class="info-body">
                        <p class="location">地点：{{currentInfo.address || ''}}</p>
                        <p class="description">描述：{{currentInfo.description || ''}}</p>
                        <p class="participants">参与人数：{{(currentInfo.participants || []).length}}人</p>
                        <div class="join-btn" v-if="currentInfo.canJoin" @click="handleJoin">
                            <span class="iconfont icon-jia"></span>
                            加入
                        </div>
                        <p class="status" v-else>
                            {{ currentInfo.time && new Date(currentInfo.time) > new Date() ? '已加入' : '已结束' }}
                        </p>
                    </div>
                </div>
            </div>
            <!-- 添加多人信息窗体 -->
            <div class="info-window" v-show="showInfo && infoType === 'classmates'" :style="infoPosition">
                <div class="info-content">
                    <div class="info-header">
                        <p class="name">{{(currentInfo?.classmates || []).length}}位同学</p>
                        <span class="iconfont icon-guanbi" @click="closeInfo"></span>
                    </div>
                    <div class="info-body">
                        <div class="classmate-list">
                            <div v-for="classmate in (currentInfo?.classmates || [])" 
                                 :key="classmate.id" 
                                 class="classmate-item">
                                <p class="name">{{classmate.name || ''}}</p>
                                <p class="location">当前位置：{{classmate.address || ''}}</p>
                                <p class="last-login">最后登录：{{classmate.lastLogin}}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 修改多个小聚信息窗体 -->
            <div class="info-window" v-show="showInfo && infoType === 'meetings'" :style="infoPosition">
                <div class="info-content">
                    <div class="info-header">
                        <p class="name">{{currentInfo.position}} - {{(currentInfo.meetings || []).length}}个小聚</p>
                        <span class="iconfont icon-guanbi" @click="closeInfo"></span>
                    </div>
                    <div class="info-body">
                        <div class="meeting-list">
                            <div v-for="meeting in (currentInfo.meetings || [])" 
                                 :key="meeting.id" 
                                 class="meeting-item">
                                <p class="time">时间：{{dataTwo(meeting.time)}}</p>
                                <p class="description">描述：{{meeting.description}}</p>
                                <p class="participants">参与人数：{{(meeting.participants || []).length}}人</p>
                                <div class="join-btn" v-if="meeting.canJoin" @click="handleJoin(meeting)">
                                    <span class="iconfont icon-jia"></span>
                                    加入
                                </div>
                                <p class="status" v-else>
                                    {{ new Date(meeting.time) > new Date() ? '已加入' : '已结束' }}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="bottom-buttons">
                <div class="button-item" @click="showClassmates">
                    <p>同学</p>
                </div>
                <div class="button-item" @click="showMeetings">
                    <p>聚会</p>
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
/* eslint-disable no-undef */
import { mapApi, gatheringApi } from '@/api/index'
import { ElMessage } from 'element-plus'
import joinClassVue from '@/components/JoinClass.vue'
import newClassVue from '@/components/NewClass.vue'
import manageClassVue from '@/components/ManageClass.vue'
import { dataTwo } from '@/utils/methods'

export default {
    name: 'MapView',
    inject: ['reload'],
    components: {
    joinClassVue,
        newClassVue,
        manageClassVue,
    },
    data() {
        return {
            dataTwo,
            addBottom: 50, // add按钮的bottom距离变量
      isJoining: 0,
            isCreating: 0,
            isManaging: 0,
            map: null,
            mapLoaded: false,
            showInfo: false,
            infoType: '',
            infoPosition: {
                left: '0px',
                top: '0px'
            },
            currentInfo: {
                id: null,
                time: '',
                address: '',
                description: '',
                participants: [],
                canJoin: false,
                classmates: [],
                meetings: [],
                position: ''
            },
            searchText: '',
            currentCity: '',
            // 热门城市数据 - 这个可能需要后端提供或者保持静态
            hotCities: [
                {
                    name: '武汉',
                    center: [114.3162, 30.5810],
                    classmateCount: 15,
                    bounds: {
                        southwest: [114.0162, 30.2810],
                        northeast: [114.6162, 30.8810]
                    }
                },
                // ... 其他城市数据
            ],
            classmatesData: [], // 改为空数组，通过API获取
            meetingsData: [], // 改为空数组，通过API获取
            markers: [],
            classId: this.$store.state.classId,  // 添加默认班级ID
            userId: this.$store.state.user.id,    // 添加默认用户ID
        }
    },
    async created() {
        // 不在 created 中加载数据
        try {
            await this.loadMap()
            this.initMap()
            // 地图初始化完成后再加载数据
            await this.fetchData()
        } catch (error) {
            console.error('地图初始化失败:', error)
            ElMessage.error('地图加载失败，请刷新页面重试')
        }
        
    },
    async mounted() {

    },
    unmounted() {
        if (this.map) {
            this.map.destroy();
            this.map = null;
            console.log("==================destroyed==================");
        }

    },
    computed: {
        mapRoute() {
            return this.$route.path;
        }
    },
    watch: {
        mapRoute() {
            if (this.$route.path !== "/map" && this.map) {
                this.map.destroy();
                this.map = null;
                // this.mapModule.dispose();
                // this.mapModule = null;
                console.log("==================destroyed==================");
            } 
        }
    },
    methods: {
        async classSwitched() {

            this.reload();
        },
        // 修改地图加载方法
        loadMap() {
            return new Promise((resolve, reject) => {
                window._AMapSecurityConfig = {
                    securityJsCode: '6d12bf061cff8eed31bc4fecfae3acfe'
                }
                if (window.AMap) {
                    resolve(window.AMap)
                    return
                }
                const script = document.createElement('script')
                script.type = 'text/javascript'
                script.src = `https://webapi.amap.com/maps?v=2.0&key=bd19e1d76e3b9b9724fd409221845cb9&plugin=AMap.ControlBar,AMap.Geocoder`
                script.onload = () => {
                    this.mapLoaded = true
                    resolve(window.AMap)
                }
                script.onerror = () => {
                    reject(new Error('高德地图加载失败'))
                }
                document.head.appendChild(script)
            })
        },
        // 获取数据的方法
        async fetchData() {
            try {
                // const classId = 1  // 直接使用 1，不使用 this.classId
                // 添加加载提示
                const loading = ElMessage({
                    message: '正在获取位置数据...',
                    duration: 0,
                    type: 'info'
                })
                let retryCount = 0;
                const maxRetries = 3;
                let locations = [];
                while (retryCount < maxRetries) {
                    try {
                        // 只获取地址数据
                        locations = await mapApi.getClassmatesLocations(this.classId);
                        console.log('Locations data:', locations);
                        if (locations.length > 0) {
                            break;
                        }
                        retryCount++;
                        await new Promise(resolve => setTimeout(resolve, 1000));
                    } catch (error) {
                        console.error(`Attempt ${retryCount + 1} failed:`, error);
                        retryCount++;
                        if (retryCount === maxRetries) {
                            throw error;
                        }
                        await new Promise(resolve => setTimeout(resolve, 1000));
                    }
                }
                // 关闭加载提示
                loading.close();
                if (locations.length > 0) {
                    // 处理位置数据
                    this.classmatesData = locations.map(item => {
                        // 从地址中提取城市名来设置默认经纬度
                        const defaultLocations = {
                            '武汉': [114.3162, 30.5810],
                            '北京': [116.4074, 39.9042],
                            '上海': [121.4737, 31.2304],
                            '深圳': [114.0579, 22.5431],
                            '光谷': [114.4198, 30.5050],
                            '武昌': [114.3162, 30.5810],
                            '洪山': [114.3418, 30.5000],
                            '江汉': [114.2709, 30.6140]
                        }
                        let location = defaultLocations['武汉']; // 默认武汉
                        for (let city in defaultLocations) {
                            if (item.最新位置信息 && item.最新位置信息.includes(city)) {
                                location = defaultLocations[city];
                                break;
                            }
                        }
                        return {
                            id: item.同学ID,
                            name: item.姓名 || `同学${item.同学ID}`,
                            address: item.最新位置信息,
                            lastLogin: item.最后登录时间,
                            longitude: location[0],
                            latitude: location[1]
                        }
                    });
                    // 显示同学位置
                    this.showClassmates();
                } else {
                    ElMessage.warning('没获取到位置数据');
                }
            } catch (error) {
                console.error('获取数据失败:', error)
                ElMessage.error('获取位置数据失败，请检查网络连接')
                this.classmatesData = []
            }
        },
        // 修改地理编码方法
        async getLocationByAddress(address) {
            return new Promise((resolve) => {
                if (!address) {
                    resolve([114.3162, 30.5810]); // 默认武汉
                    return;
                }
                const geocoder = new AMap.Geocoder({
                    city: "全国" // 城市，默认："全国"
                });
                geocoder.getLocation(address, (status, result) => {
                    if (status === 'complete' && result.info === 'OK') {
                        const location = result.geocodes[0].location;
                        resolve([location.lng, location.lat]);
                    } else {
                        console.warn('地理编码失败:', address);
                        resolve([114.3162, 30.5810]); // 默认武汉
                    }
                });
            });
        },
        // 修改显示同学位置方法
        async showClassmates() {
            /*
            if (!this.mapLoaded || !window.AMap) {
                console.error('地图未加载完成')
                return
            }*/
            this.clearMarkers()
            console.log('Showing classmates:', this.classmatesData)
            // 按位置分组
            const locationGroups = {};
            // 先对同学按位置分组
            for (const classmate of this.classmatesData) {
                const address = classmate.address;
                if (!locationGroups[address]) {
                    locationGroups[address] = [];
                }
                locationGroups[address].push(classmate);
            }
            // 为每个位置创建标记
            for (const address in locationGroups) {
                try {
                    const classmates = locationGroups[address];
                    const isMultiple = classmates.length > 1;
                    // 使用地理编码获取位置
                    const location = await this.getLocationByAddress(address);
                    console.log(`Got location for ${address}:`, location);
                    const marker = new AMap.Marker({
                        position: new AMap.LngLat(location[0], location[1]),
                        icon: new AMap.Icon({
                            size: new AMap.Size(40, 40),
                            image: isMultiple ? 
                                'https://webapi.amap.com/theme/v1.3/markers/n/mark_r.png' : 
                                'https://webapi.amap.com/theme/v1.3/markers/n/mark_b.png'
                        }),
                        title: isMultiple ? `${classmates.length}位同学` : classmates[0].name,
                        label: isMultiple ? {
                            content: `${classmates.length}`,
                            direction: 'center',
                            offset: new AMap.Pixel(0, 0),
                            style: {
                                color: '#fff',
                                fontSize: '12px',
                                fontWeight: 'bold'
                            }
                        } : null
                    });
                    marker.on('click', (e) => {
                        if (isMultiple) {
                            // 显示多人信息窗体
                            this.infoType = 'classmates';
                            this.currentInfo = {
                                classmates
                            };
                        } else {
                            // 显示单人信息窗体
                            this.infoType = 'classmate';
                            this.currentInfo = classmates[0];
                        }
                        this.infoPosition = {
                            left: e.pixel.x + 20 + 'px',
                            top: e.pixel.y - 0 + 'px'
                        };
                        this.showInfo = true;
                    });
                    this.map.add(marker);
                    this.markers.push(marker);
                    console.log(`Added ${isMultiple ? 'group' : 'single'} marker at:`, location);
                } catch (error) {
                    console.error('处理位置标记失败:', error, address);
                }
            }
            // 调整地图视野以包含所有标记点
            if (this.markers.length > 0) {
                this.map.setFitView(this.markers);
            }
        },
        // 修改显示小聚位置方法
        async showMeetings() {
            /*
            if (!this.mapLoaded || !window.AMap) {
                console.error('地图未加载完成')
                return
            }*/

            try {
                // 获取聚会数据
                const params = {
                    class_id: this.$store.state.classId,
                    user_id: this.$store.state.user.id
                }
                const gatheringsRes = await gatheringApi.getGatheringList(params)
                console.log('Gatherings response:', gatheringsRes)

                if (!gatheringsRes || !Array.isArray(gatheringsRes)) {
                    console.warn('No gatherings data received')
                    this.meetingsData = []
                    return
                }

                // 按位置分组聚会
                const locationGroups = {};
                
                // 处理聚会数据并按位置分组
                for (const meeting of gatheringsRes) {
                    const position = meeting.position;
                    if (!locationGroups[position]) {
                        locationGroups[position] = [];
                    }
                    locationGroups[position].push({
                        id: meeting.gaID,
                        time: meeting.date,
                        address: meeting.position,
                        description: meeting.discription,
                        proposer: meeting.proposer,
                        classId: meeting.classID,
                        canJoin: new Date(meeting.date) > new Date() && !meeting.participant?.includes(this.userId),
                        participants: meeting.participant || []
                    });
                }

                // 清除现有标记
                this.clearMarkers()

                // 创建地理编码服务实例
                const geocoder = new AMap.Geocoder({
                    city: "全国"
                });

                // 使用 Promise.all 处理所有位置
                const locationPromises = Object.entries(locationGroups).map(([position, meetings]) => {
                    return new Promise((resolve) => {
                        geocoder.getLocation(position, (status, result) => {
                            if (status === 'complete' && result.info === 'OK') {
                                const location = result.geocodes[0].location;
                                console.log(`Got location for ${position}:`, location);
                                
                                const isMultiple = meetings.length > 1;
                                const marker = new AMap.Marker({
                                    position: new AMap.LngLat(location.lng, location.lat),
                                    icon: new AMap.Icon({
                                        size: new AMap.Size(40, 40),
                                        image: isMultiple ? 
                                            'https://webapi.amap.com/theme/v1.3/markers/n/mark_r.png' : 
                                            'https://webapi.amap.com/theme/v1.3/markers/n/mark_bs.png'
                                    }),
                                    title: isMultiple ? `${meetings.length}个小聚` : meetings[0].description,
                                    label: isMultiple ? {
                                        content: `${meetings.length}`,
                                        direction: 'center',
                                        offset: new AMap.Pixel(0, 0),
                                        style: {
                                            color: '#fff',
                                            fontSize: '12px',
                                            fontWeight: 'bold'
                                        }
                                    } : null
                                });

                                marker.on('click', (e) => {
                                    if (isMultiple) {
                                        this.infoType = 'meetings';
                                        this.currentInfo = {
                                            meetings,
                                            position
                                        };
                                    } else {
                                        this.infoType = 'meeting';
                                        this.currentInfo = meetings[0];
                                    }
                                    this.infoPosition = {
                                        left: e.pixel.x + 20 + 'px',
                                        top: e.pixel.y - 0 + 'px'
                                    };
                                    this.showInfo = true;
                                });

                                this.map.add(marker);
                                this.markers.push(marker);
                                console.log(`Added ${isMultiple ? 'group' : 'single'} meeting marker at:`, location);
                            } else {
                                console.warn(`地理编码失败: ${position}`, status);
                            }
                            resolve();
                        });
                    });
                });

                // 等待所有位置处理完成
                await Promise.all(locationPromises);
                console.log('All markers added:', this.markers.length);

                // 调整地图视野以包含所有标记点
                if (this.markers.length > 0) {
                    this.map.setFitView(this.markers);
                }
            } catch (error) {
                console.error('获取小聚数据失败:', error);
                ElMessage.error('获取小聚数据失败');
                this.meetingsData = [];
            }
        },
        // 修改加入聚会方法
        async handleJoin() {
            try {
                const params = {
                    gathering_id: this.currentInfo.id,
                    user_id: this.userId  // 从 vuex 获取当前用户ID
                }
                console.log("加入聚会",params)
                await gatheringApi.joinGathering(params)
                //ElMessage.success('成功加入聚会')
                this.$message({ type: "success", message: "成功加入聚会！" });
                // 加入成功后刷新数据
                await this.showMeetings()
                // 关闭信息窗口
                this.closeInfo()
            } catch (error) {
                console.error('加入聚会失败:', error)
                ElMessage.error('加入聚会失败，请重试')
            }
        },
        // 清所有标记
        clearMarkers() {
            this.markers.forEach(marker => {
                this.map.remove(marker)
            })
            this.markers = []
            this.showInfo = false
        },
        // 处理搜索
        handleSearch() {
            if (this.searchText) {
                const city = this.hotCities.find(city => 
                    city.name.includes(this.searchText)
                )
                if (city) {
                    this.selectCity(city)
                }
            }
        },
        // 选择城市
        selectCity(city) {
            this.currentCity = city.name
            this.map.setCenter(city.center)
            this.map.setZoom(12)
            // 如果有标记点，重新筛选显示
            if (this.markers.length > 0) {
                this.showClassmates()
            }
        },
        closeInfo() {
            this.showInfo = false;
            this.infoType = '';
            // 重置 currentInfo 到初始状态
            this.currentInfo = {
                id: null,
                time: '',
                address: '',
                description: '',
                participants: [],
                canJoin: false,
                classmates: [],
                meetings: [],
                position: ''
            };
        },
        // 添加 initMap 方
        initMap() {
            try {
                // 创建一个离屏 canvas
                const offscreenCanvas = document.createElement('canvas')
                offscreenCanvas.setAttribute('willReadFrequently', 'true')
                // 创建地图实例
                this.map = new AMap.Map('container', {
                    viewMode: '3D',
                    zoom: 4,
                    center: [116.397428, 39.90923],
                    pitch: 45,
                    mapStyle: 'amap://styles/whitesmoke',
                    features: ['bg', 'building', 'point'],
                    showBuildingBlock: true,
                    // 使用离屏 canvas
                    canvas: {
                        createElement: () => {
                            const canvas = document.createElement('canvas')
                            canvas.setAttribute('willReadFrequently', 'true')
                            return canvas
                        }
                    }
                })
                // 添加控制条
                this.map.addControl(new AMap.ControlBar({
                    position: {
                        right: '10px',
                        top: '10px'
                    }
                }))
                // 修改点击事件处理
                this.map.on('click', (e) => {
                    const geocoder = new AMap.Geocoder({
                        radius: 1000,
                        extensions: 'all'
                    })
                    const lnglat = [e.lnglat.getLng(), e.lnglat.getLat()]
                    geocoder.getAddress(lnglat, (status, result) => {
                        if (status === 'complete') {
                            if (result.regeocode) {
                                const address = result.regeocode.formattedAddress
                                if (address && address.trim()) {
                                    if (confirm(`是否将当前位置更新为: ${address}？`)) {
                                        // 修改这里：传入纬度参数
                                        this.updateUserLocation(address)
                                    }
                                } else {
                                    alert('无法获取该位置的地址信')
                                }
                            } else {
                                alert('该位置暂无地址信息')
                            }
                        } else {
                            console.error('地理编码失败:', status)
                            alert('获取地址信息失败，请重试')
                        }
                    })
                })
            } catch (error) {
                console.error('地图初始化失败:', error)
            }
        },
        // 修改更新位置方法
        async updateUserLocation(address) {
            try {
                const userId = this.userId
                console.log('Updating location:', {address, userId })
                // 添加加载提示
                const loading = ElMessage({
                    message: '正在更新位置...',
                    duration: 0,
                    type: 'info'
                })
                // 更新地址
                const locationRes = await mapApi.updateLocation({
                    new_add: address,
                    user_id: this.userId
                })
                console.log('Location update response:', locationRes)
                // 关闭加载提示
                loading.close()
                // 添加成功提示
                ElMessage({
                    message: '位置更新成功',
                    type: 'success'
                })
                // 更新成功后刷新数据
                await this.fetchData()
            } catch (error) {
                console.error('更新位置失败:', error)
                ElMessage({
                    message: '更新位置失败，请重试',
                    type: 'error',
                    duration: 5000
                })
            }
        },
        // 添加日期格式化方法
        formatDate(dateString) {
            if (!dateString) return '';
            const date = new Date(dateString);
            return date.toLocaleString('zh-CN', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit'
            });
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
.map-view {
    min-height: calc(100vh - 222px);
    padding-top: 258px;
    display: flex;
    width: 100%;
    .sidebar {
        width: 300px;
        // background: rgba(255,255,255,0.9);
        // backdrop-filter: blur(10px);
        border-right: 1px solid rgba(0,0,0,0.1);
        padding: 20px 20px 20px 20px;
        box-sizing: border-box;
        overflow-y: auto;
        .search-box {
            position: relative;
            margin-bottom: 20px;
            .search-input {
                width: 100%;
                height: 36px;
                border: 1px solid @gray-2;
                border-radius: 18px;
                padding: 0 40px 0 16px;
                box-sizing: border-box;
                font-size: @size-14;
                &:focus {
                    border-color: @primary-color;
                }
            }
            .icon-sousuo {
                position: absolute;
                right: 12px;
                top: 50%;
                transform: translateY(-50%);
                color: @gray-2;
                font-size: 18px;
            }
        }
        .hot-cities {
            margin-top: 20px;
            .title {
                font-size: @size-14;
                color: @gray-2;
                margin-bottom: 12px;
            }
            .city-list {
                .city-item {
                    padding: 16px;
                    border-radius: 8px;
                    margin-bottom: 12px;
                    background: rgba(255,255,255,0.6);
                    cursor: pointer;
                    transition: @tr;
                    &:hover {
                        background: rgba(255,255,255,0.9);
                        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                    }
                    &.active {
                        background: rgba(59,115,240,0.1);
                        border: 1px solid @primary-color;
                    }
                    .city-name {
                        font-size: @size-16;
                        color: @gray-0;
                        margin-bottom: 4px;
                    }
                    .city-count {
                        font-size: @size-12;
                        color: @primary-color;
                        margin-top: 4px;
                    }
                }
            }
        }
    }
    .main-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        .map-container {
            width: 100%;
            height: calc(100% - 20px);
        }
        .bottom-buttons {
            height: 60px;
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 100px;
            // background: rgba(255,255,255,0.9);
            // backdrop-filter: blur(10px);
            border-top: 1px solid rgba(0,0,0,0.1);
            .button-item {
                display: flex;
                flex-direction: column;
                align-items: center;
                cursor: pointer;
                padding: 10px 20px;
                border-radius: 8px;
                transition: @tr;
                &:hover {
                    background: rgba(59,115,240,0.1);
                }
                .iconfont {
                    font-size: 24px;
                    color: @primary-color;
                    margin-bottom: 4px;
                }
                p {
                    font-size: @size-14;
                    color: @gray-0;
                }
            }
        }
    }
    .info-window {
        position: absolute;
        z-index: 100;
        background: rgba(255,255,255,0.9);
        backdrop-filter: blur(10px);
        border-radius: 8px;
        box-shadow: 0 2px 12px rgba(0,0,0,0.1);
        padding: 16px;
        min-width: 240px;
        .info-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 12px;
            .name, .time {
                color: @gray-0;
                font-size: @size-14;
                font-weight: 500;
            }
            .icon-guanbi {
                cursor: pointer;
                color: @gray-2;
                font-size: 16px;
                &:hover {
                    color: @gray-1;
                }
            }
        }
        .info-body {
            .location, .description, .last-login {
                color: @gray-1;
                font-size: @size-14;
                margin-bottom: 8px;
                line-height: 1.5;
            }
            .status {
                color: @gray-2;
                font-size: @size-14;
            }
            .join-btn {
                display: inline-flex;
                align-items: center;
                padding: 6px 12px;
                background: @primary-color;
                color: white;
                border-radius: 4px;
                cursor: pointer;
                transition: @tr;
                .icon-jia {
                    margin-right: 4px;
                    font-size: 12px;
                }
                &:hover {
                    opacity: 0.9;
                }
            }
        }
        .classmate-list {
            max-height: 200px;
            overflow-y: auto;
            .classmate-item {
                padding: 8px 0;
                border-bottom: 1px solid rgba(0,0,0,0.1);
                &:last-child {
                    border-bottom: none;
                }
                .name {
                    font-size: @size-14;
                    color: @gray-0;
                    margin-bottom: 4px;
                }
                .last-login {
                    font-size: @size-12;
                    color: @gray-2;
                }
            }
        }
        .meeting-list {
            max-height: 300px;
            overflow-y: auto;
            .meeting-item {
                padding: 12px;
                border-bottom: 1px solid rgba(0,0,0,0.1);
                &:last-child {
                    border-bottom: none;
                }
                .time {
                    font-size: @size-14;
                    color: @gray-0;
                    margin-bottom: 4px;
                }
                .description {
                    font-size: @size-14;
                    color: @gray-1;
                    margin-bottom: 4px;
                }
                .participants {
                    font-size: @size-12;
                    color: @gray-2;
                    margin-bottom: 8px;
                }
                .join-btn {
                    display: inline-block;
                    padding: 4px 12px;
                    background-color: @primary-color;
                    color: #fff;
                    border-radius: 4px;
                    cursor: pointer;
                    &:hover {
                        background-color: darken(@primary-color, 10%);
                    }
                    .icon-jia {
                        margin-right: 4px;
                    }
                }
                .status {
                    font-size: @size-12;
                    color: @gray-2;
                }
            }
        }
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
</style> 
