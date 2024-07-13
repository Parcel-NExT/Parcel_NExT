# Legends of Sprint and Autumn (春秋战记)

Tags: Parcel, Package, MiniGame, #Official, Project Nine, RPG, Strategy, Text-based, #Visual, #Graphical, Graphical Novel, Game
Genre: Turn Based Strategy
Theme: Classical/Historical Strategy

## Table of Contents

* Introduction

## Introduction

### Story Background

九层楼世界（日本）战国时期，带点传奇/魔幻色彩（体现在具体人物上）。世界背景设定参考九层楼，游戏具体设定在此文档和见具体实现代码。

### Publishing

Published by Methodox.io itself. Base game Methodox Parcel NExT is free.

Sold for $3.5 on Steam (and Itch.io) as Parcel Package (add-on/DLC). (Including runtime dynamic skin change on supported front-ends)

Notice Steam version of Parcel NExT has Steam API built-in and uses Steam's system for updating etc. and is connected to a Steam account, and features Steam achievemtns, profile, etc. It's supportef by both Neo and Gospel. It uses a different codebase compared with non-steam version.

## Chapter 1: Gameplay
* 搜集历史人物
* 战役

## References

Original story: See Project Nine world building designs and story novels.

Game style reference:

* 早期街机和PC的三国志游戏

# Notes

## 20240713

### Early Conception - Form Factor and Succinct Conception

* (Name) 多国志（单机策略）
* Built-in package game。回合制国家管理；使用Node进行query和行为。
* 使用Query node进行当前世界和自己国家政治环境和资源的query。
* 使用Action node进行具体行为布局。
* 一切Ready了执行“Next Season”。
* 游戏自动保存（数据SQLite），也可以手动保存到具体文件，或者configure保存路径。尽量隐藏别国和世界数据，只保存玩家个人数据。
* 游戏数据包裹表单、图表、还有图片RPG素材。
* 游戏完全实现需要document级别meta-programming、完善的制图（地图、数据报表等），以及multimedia playback功能包括音效和短片。