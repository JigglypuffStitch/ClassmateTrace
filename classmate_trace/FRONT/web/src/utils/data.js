// 页面性质
export const wallType=[
    {
        name: '留言墙',
        slogan: '每一句话都是青春的足迹。', 
    },
    {
        name: '照片墙',
        slogan: '每一张图都是同窗的记忆。',
    },
    {
        name: '回忆录',
        slogan: '足迹与记忆连成你我回忆。',
    }
];

// 分类标签、
export const label = [
    ['过去','将来','目标','理想','爱情','亲情','友情','秘密','无题'],
    ['我们','我','ta','值得纪念的','生活','工作','天空','无题']
]

// 卡片背景颜色
export const cardColor = [
    'rgba(252,175,162,0.30)',
    'rgba(255,227,148,0.30)',
    'rgba(146,230,245,0.30)',
    'rgba(168,237,138,0.30)',
    'rgba(202,167,247,0.30)',
    'rgba(212,212,212,0.30)' // 其他处用的隐藏颜色
]

// 卡片背景颜色: 1透明度
export const cardColor1 = [
    'rgba(252,175,162,1)',
    'rgba(255,227,148,1)',
    'rgba(146,230,245,1)',
    'rgba(168,237,138,1)',
    'rgba(202,167,247,1)' // 小矩形块用的颜色，不可选择隐藏颜色
]

// 头像背景 - 14个
export const portrait = [
    'linear-gradient(180deg, #FFA9D9 0%, #E83D3D 100%)',
    'linear-gradient(180deg, #FFA7EB 0%, #F026A8 100%)',
    'linear-gradient(180deg, #F5A8FF 0%, #BF23E5 100%)',
    'linear-gradient(180deg, #DFA1FF 0%, #9A36F0 100%)',
    'linear-gradient(180deg, #C9AAFF 0%, #6D3CF5 100%)',
    'linear-gradient(180deg, #9EAAFF 0%, #3846F4 100%)',
    'linear-gradient(180deg, #8CD8FF 0%, #2A6AF0 100%)',
    'linear-gradient(180deg, #7BE7FF 2%, #1E85E2 100%)',
    'linear-gradient(180deg, #92FDFF 0%, #14B2DD 100%)',
    'linear-gradient(180deg, #89FED8 0%, #18C997 100%)',
    'linear-gradient(180deg, #D7FFA7 0%, #5ED52A 100%)',
    'linear-gradient(180deg, #FFED48 0%, #FD9E16 100%)',
    'linear-gradient(180deg, #FFDC83 0%, #F88816 100%)',
    'linear-gradient(180deg, #FFBA8D 1%, #EB6423 100%)',
]

// 空状态显示
export const none = [
    {
        url: require('../assets/images/card.svg'),
        msg: '还没有留言，快贴上第一张吧',
    },
    {
        url: require('../assets/images/photo.svg'),
        msg: '还没有照片，快贴上第一张吧',
    },
    {
        url: require('../assets/images/memoir.svg'),
        msg: '还没有回忆，请等待更多照片的上传'
    }
]