# Readme

- 现在还在看教程💦
- `Unity` 坑好多呀呜呜
- 为什么元骑的贴图要留空白……为什么……
  - ~~增加剽窃难度~~
- *达成成就* : $Deadline$ 战士！

## 物体
- 类型
- ID
- 碰撞体数据

## 武器
- 名称:name,string
- 贴图:sprite,string
- 消耗EP:EP,float
- 是否消耗,bool
- 剩余个数,int
- 攻击类型,依据类型判断
  - 近战(Sword)
    - 攻击特效
    - 范围
      - 扇形角度,float
      - 距离,float
    - 伤害,float
  - 发射子弹(Gun)
    - 攻击特效
    - 子弹（统一`Prefab`）
      - 贴图,string
      - 速度,float
      - 伤害,float
      - 飞行动画
      - 碰撞特效
    - 范围（扇形角度+个数）float,int

## 敌人
- 名称, string
- 贴图, string
- 动画, string
- 生命数据, HealthData
- 默认状态机入口, Type