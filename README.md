# IMP_VR
202221104 강희진<br>
202221092 박수현<br>
202221046 배민우<br>
202322171 권혜원

## (1/3) Issue Convention
### ✅기능 이슈
* 이슈 작성 후 작업 진행 바랍니다.
* 체크리스트에 체크, 완료되면 Close 해주세요.<br>
> [Feat] KHJ_UI<br>
> * Description<br>
> WBP 제작 및 코드 작성<br>
> * To Do<br>
> 시작 UI<br>
> 엔딩 UI
### 🙏요청 사항
* 제목에 ```[요청 대상] 이슈 이름```을 작성
* ```요청 대상```은 이슈 확인 시 Comment를 남긴 후 체크리스트에 체크, 완료되면 Close 해주세요.
> [희진] 에셋 위치 조정 필요<br>
> * Description<br>
> PSH 폴더 B2_Map을 참고하시고 에셋 크기와 위치를 맞춰주시면 감사하겠습니다.<br>
> * To Do<br>
> 키패드 크기 줄이기<br>
> 컬러 다이얼 크기 키우기

## (2/3) Commit Message Convention
> 브랜치명: 이니셜_이름 (예: ```KHJ_Proto```)<br>
> 작은 변경사항이라도 꼭 중간마다 커밋, 푸쉬 부탁드립니다.
* ```[Feat]``` : 새로운 기능 구현
* ```[Add]``` : Feat 이외의 부수적인 코드, 라이브러리 추가, 새로운 View나 Activity 생성
* ```[Fix]``` : 버그, 오류 해결
* ```[!HOTFIX]``` : 치명적인 버그 해결
* ```[Refactor]``` : 코드의 내부 구조를 개선하지만 외부 동작은 변경하지 않는 경우(코드 중복 제거, 메서드 추출, 클래스 분리, 변수/함수명 변경, 코드 최적화, 아키텍처 개선)
* ```[Comment]``` : 주석 추가 및 변경
* ```[Chore]``` : 자잘한 수정에 대한 커밋
* ```[Del]``` : 쓸모없는 코드 삭제
* ```[Remove]``` : 파일 삭제
* ```[Test]``` : 테스트 코드 추가
* ```[Design]``` : 화면 디자인에 관한 수정
* ```[Docs]``` : README, wiki 등 문서 수정
> [Fix] callback error<br>
> [Feat] google login<br>
> [Add] LoginActivity<br>
> [Design] iphone 12 레이아웃 조정<br>
> [Remove] 중복 파일 삭제<br>
> [Comment] 메인 뷰컨 주석 추가<br>

## (3/3) Naming Convention
#### 1. 폴더명
* 유니티 Assets 폴더 내에 각자 ```이니셜``` 폴더를 만들어 작업 진행
  * 폴더 이름은 ```PascalCase```, 공백과 유니코드 문자 및 기타 기호 사용 금지
  * 개인 이니셜 폴더 내에 Scripts, Scenes, Prefabs, Materials, Models, Sounds 폴더 만들어 파일 정리
  
#### 2. 유니티 에셋 이름

|Prefix/Suffix|내용|예시|
|------|---|---|
|PF_|Prefab|PF_Player|
|UI_|UI 요소|UI_Start|
|AC_|Animation Controller|AC_Player|
|AM_|Animation|AM_Run|
|SM_|State Machine|SM_PlayerMovement|
|M_|Material|M_Transparent|
|SPR_|Sprite|SPR_Arrow|
|SFX_|Sound Effect|SFX_Enter|
|BGM_|Background Music|BGM_Main|
|AT_|Audio Mixer|AT_Master|
|F_|Font|F_NotoSans|
|RT_|Render Texture|RT_CameraView|
|ML_|Marker Library|ML_Currency|

#### 3. C# 변수명
  * Public 멤버 변수는 파스칼 케이스 사용 (예: Public int PlayerNum;)
  * Non Public 멤버 변수는 _카멜 케이스 사용 (예: private int _playerNum;)
  * 지역 변수, 함수의 매개변수는 카멜 케이스 사용 (예: int playerScore;)
  * 부울 변수는 질문형으로 작성 (예: isActive, canJump)
  * const 변수는 대문자와 언더바 사용 (예: MAX_PLAYER_COUNT)
  * 열거형(Enum): 파스칼 케이스 사용 (타입: CurrencyType, 값: Coin100, Bill1000)

#### 4. C# 함수명
* 함수는 파스칼 케이스 사용 (예: StartGame())
* Bool을 반환하는 함수는 질문형으로 작성 (예: IsPlayerAlive(), CanPickupItem())
* 이벤트 핸들러와 콜백 함수는 On으로 시작 (예: OnPlayerDeath(), OnCoinCollected(), OnARMarkerDetected())
