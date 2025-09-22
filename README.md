# 챙김이 (Chaenggimi) - 분실물 예방 및 관리 앱

[![.NET MAUI](https://img.shields.io/badge/.NET-MAUI-purple.svg)](https://dotnet.microsoft.com/apps/maui)
[![Firebase](https://img.shields.io/badge/Firebase-orange.svg)](https://firebase.google.com/)

2025년 신한대학교 SW중심대학 앱개발 경진대회 출품작입니다.

## 🧐 프로젝트 소개 (About)

**챙김이**는 교내 구성원들의 잦은 소지품 분실 문제를 해결하기 위해 개발된 안드로이드 앱입니다. 단순히 잃어버린 물건을 찾아주는 기존의 수동적인 방식을 넘어, **'사전 예방'**이라는 새로운 접근을 통해 사용자의 좋은 습관을 만들어주는 것을 목표로 합니다.

## ✨ 주요 기능 (Features)

* **실시간 분실물 게시판**
    * Firebase Realtime Database와 연동하여 '분실' 또는 '습득'된 물품의 목록을 모든 사용자가 실시간으로 공유합니다.
    * 항목을 탭하여 사진, 장소, 연락처 등 상세 정보를 확인할 수 있습니다.

* **'내 소지품' 관리**
    * 자주 사용하는 개인 소지품을 사진과 함께 핸드폰 내부에 안전하게 저장하고 관리할 수 있습니다.
    * 분실물 등록 시, '내 소지품' 목록에서 아이템을 선택하면 사진과 이름이 자동으로 입력되어 신고 과정을 단축시켜 줍니다.

* **'챙김이' 맞춤 알림 (핵심 기능)**
    * 사용자가 지정한 시간에 매일 푸시 알림을 보냅니다.
    * '내 소지품'에 등록된 아이템 이름을 활용하여 "오늘은 '에어팟' 잘 챙기셨나요?"와 같이 개인화된 메시지를 보내 분실 예방 효과를 극대화합니다.

* **사용자 친화적 UI/UX**
    * 하단 탭 메뉴로 핵심 기능을 쉽게 오갈 수 있습니다.
    * '분실/습득' 모드에 따라 UI가 동적으로 변경되어 사용자의 편의성을 높였습니다.
    * 데이터 처리 시 로딩 인디케이터를 표시하여 명확한 시각적 피드백을 제공합니다.

## 🛠️ 사용 기술 (Tech Stack)

* **프레임워크**: .NET MAUI (C#, XAML)
* **데이터베이스**: Firebase Realtime Database
* **파일 저장소**: Firebase Storage
* **로컬 저장소**: .NET MAUI Preferences API
* **알림**: Plugin.LocalNotification

## 🚀 시작하기 (Getting Started)

이 프로젝트를 직접 실행해보려면 아래의 과정이 필요합니다.

### **1. 소스 코드 복제**

```bash
git clone [https://github.com/haechanmoon/app_contest.git](https://github.com/haechanmoon/app_contest.git)
