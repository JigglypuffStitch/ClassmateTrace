<template>
  <div>
      <video src="@/assets/images/bg1.mp4" autoplay="autoplay" muted="muted" loop="loop" class="bg-video"> </video>
      <div class="centered">
        <img src="@/assets/images/classmaps_logo_admin.png" class="logo-img"/>
        <h1 class="text">用户信息显示</h1>
      </div>
      <div id="Gender_Number" style="width:1000px;height:500px;margin: -70px auto;"></div>
      <div id="User_Number" style="width:1000px;height:500px;margin:30px auto;"></div>     
  </div>
</template>

<script>
// 引入echarts
import * as echarts from 'echarts'
import {onMounted} from "vue";
// import { getCurrentInstance } from "vue";
import { ref } from 'vue'
import { adminApi } from '@/api/index'

export default {
  // data() {
  //     return {
  //       userInfoData:{
  //         maleNumberData: ref(0),
  //         femaleNumberData: ref(0),
  //         otherNumberData: ref(0),
  //         unknownNumberData: ref(0),
  //       },
        
  //     };
  // },
  
  setup() {  
    // infoData = getCurrentInstance();
    onMounted(async () => { // 需要在页面加载完毕后展示图表，这里使用Vue3的组合式生命周期钩子 onMounted()
      // this.updateInfo();
      let maleNumberData = ref(0);
    let femaleNumberData = ref(0);
    let otherNumberData = ref(0);
    let unknownNumberData = ref(0);
    // const infoData = getCurrentInstance();
    let userNumberData= ref([]);
    // 获取动态数据
    try {
      // const response = await axios.get('/api/user-data');
      // const data = response.data;
      const response = await adminApi.getUserStats();
      console.log('用户统计量加载成功:', response);
      const today = new Date();  
      response.forEach((info) => {
        console.log(info); 
        userNumberData.value.push(info.loginUserNum);
        // console.log(info.date,'----------',today);
        // let id = '';
        // let d = '';
        // id = info.date.substring(0, 10);
        // d = today.toISOString().substring(0, 10);
        // console.log(id,'----------',d);
        if (info.date.substring(0, 10) == today.toISOString().substring(0, 10)) {
          maleNumberData.value = info.maleNum;
          femaleNumberData.value = info.femaleNum;
          otherNumberData.value = info.otherNum;
          unknownNumberData.value = info.unknownNum;
          console.log('用户性别统计量加载成功:', otherNumberData.value);
        }
      });
      console.log('用户总量加载成功:', userNumberData.value);
      console.log('用户性别统计量:', otherNumberData.value);
      const User_Number = echarts.init(document.getElementById("User_Number"));
      // 设置图表配置项,可以直接从Echarts的示例中，将配置项复制下来后放入下列的myChart.setOption()中实现图表的样式更换
      User_Number.setOption({
        title: {
          text: '用户数量统计',
          left: 'center'
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'cross',
            label: {
              backgroundColor: '#6a7985'
            }
          }
        },
        legend: {
          data: ['用户日登录数量'],
          left: 'left'
        },
        toolbox: {
          feature: {
            saveAsImage: {}
          }
        },
        grid: {
          left: '3%',
          right: '4%',
          bottom: '3%',
          containLabel: true
        },
        xAxis: [
          {
            type: 'category',
            boundaryGap: false,
            data: ['1','2', '3', '4', '5', '6', '7']
          }
        ],
        yAxis: [
          {
            type: 'value'
          }
        ],
        series: [
          {
            name: '用户日登录数量',
            type: 'line',
            stack: 'Total',
            areaStyle: {},
            emphasis: {
              focus: 'series'
            },
            // 指定 dynamicUserNumberData 作为数据源
            data: userNumberData.value
            // data: [2,1,3,4,2,1,4]
          },
        ]
      });
      const Gender_Number = echarts.init(document.getElementById("Gender_Number"));
      Gender_Number.setOption({
        title: {
          text: '性别数量统计',
          left: 'center'
        },
        tooltip: {
          trigger: 'item'
        },
        legend: {
          orient: 'vertical',
          left: 'left'
        },
        series: [
          {
            name: '性别信息',
            type: 'pie',
            radius: ['40%', '70%'],
            avoidLabelOverlap: false,
            label: {
              show: false,
              position: 'center'
            },
            emphasis: {
              label: {
                show: true,
                fontSize: '30',
                fontWeight: 'bold'
              }
            },
            labelLine: {
              show: false
            },
            // 指定 dynamicGenderNumberData 作为数据源
            data: [
              { value: Number(maleNumberData.value), name: '男生' },
              { value: Number(femaleNumberData.value), name: '女生' },
              { value: Number(otherNumberData.value), name: '其他' },
              { value: Number(unknownNumberData.value), name: '未知' },
            ]
            // data: [
            //   { value: 2, name: '男生' },
            //   { value: 2, name: '女生' },
            //   { value: 2, name: '未知' },
            //   { value: 2, name: '其他' },
            // ]
          }
        ]
      });     
      window.onresize = function () {
        User_Number.resize();
        Gender_Number.resize();
      };       
    } catch (error) {
      // 处理错误
      console.error('用户统计量加载失败:',error);
    }
    });
  }    
}
</script>

<style lang="less" scoped>
.centered {
    display: flex; // 平铺
    justify-content: center; // 居中
    text-align: center;
    margin: 100px auto; /* 矫正垂直居中并增加向下的间距 */
}
.bg-video {
    height:"840";
    position: fixed; // 不随着页面滚动
    top:0;
    left:0;
    z-index:-1;
}

  .logo-img {
    display: flex;
    align-items: center;
    position: absolute; /* 绝对定位 */
    top:40px; /* 调整图片位置 */
    width: 350px; /* 缩小logo */
    filter: grayscale(30%); /* 灰色 */
    z-index: 1;
}
.text {
  padding-top: 120px;
  color: @gray-0;
}

</style>

