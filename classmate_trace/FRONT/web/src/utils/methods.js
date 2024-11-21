// 时间显示处理
export const dataOne = (e) => {
    let d = new Date(e);
    let year = d.getFullYear();
    let month = d.getMonth()+1;
    let day = d.getDate();
    if (day<10) {
        day = '0'+day;
    }
    if (month<10) {
        month = '0'+month;
    }
    let date = year+'.'+month+'.'+day;
    return date;
}

export const dataTwo = (e) => {
    let d = new Date(e);
    let year = d.getFullYear();
    let month = d.getMonth()+1;
    let day = d.getDate();
    let hours = d.getHours();
    let minutes = d.getMinutes();
    if (day<10) {
        day = '0'+day;
    }
    if (month<10) {
        month = '0'+month;
    }
    if (hours<10) {
        hours = '0'+hours;
    }
    if (minutes<10) {
        minutes = '0'+minutes;
    }
    let date = year+'.'+month+'.'+day+' '+hours+':'+minutes;
    return date;
}

// 获取图片
export const getObjectURL = (file) => {
    var url = null;
    if (window.createObjectURL != undefined) {  // basic
        url = window.createObjectURL(file);
    }
    else if (window.URL != undefined) {  // mozilla(firefox)
        url = window.URL.createObjectURL(file);
    }
    else if (window.webkitURL != undefined) {  // webkit or chrome
        url = window.webkitURL.createObjectURL(file);
    }
    return url;
}