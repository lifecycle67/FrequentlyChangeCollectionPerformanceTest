# FrequentlyChangeCollectionPerformanceTest
메인이 아닌 여러 스레드에서 잦은 빈도로 컬렉션을 변경할 때 렌더링 성능을 확인해보기 위한 프로젝트를 작성함.
20ms 마다 아이템이 추가되고 35ms마다 아이템이 삭제되는 컬렉션을 바인딩하는 5개의 ItemsControl을 배치, 1000개의 메세지를 받고 동작을 완료한다.
ItmesControl이 작업을 완료하는 시간을 측정하고 UI 업데이트 반응성을 확인한다.

## 동작 옵션
* CollectionView / ObservableCollection / BindingOperation.EnableCollectionSynchronization
* ObservableCollection / Dispatcher Invoke
* ObservableCollection / Dispatcher beginInvoke

## BindingOperation.EnableCollectionSynchronization
컬렉션에 대한 동기화된 액세스를 보장함. [BindingOperation.EnableCollectionSynchronization - MSDN](https://docs.microsoft.com/ko-kr/dotnet/api/system.windows.data.bindingoperations.enablecollectionsynchronization?view=windowsdesktop-6.0)

## DispatcherPriority
Dispatcher에 게시할 작업의 우선 순위를 설정함. Dispatcher.Input을 기준으로 UI 반응성이 크게 차이남. [DispatcherPriority Enum - MSDN](https://docs.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcherpriority?view=windowsdesktop-6.0) (마우스 포인터 위치 변경을 지속하고 있을 때)
* Dispatcher.Input 보다 높을 경우 입력 반응 느림 / 렌더링 반응 향상.
* Dispatcher.Input 보다 낮을 경우 입력 반응 빠름 / 렌더링 반응 느림. 

## ReactiveX
메세지 발생 이벤트 시퀀스를 일정 주기로 관측하고 콜백 메서드를 주 스레드에서 실행하도록 하여 동작 상태를 확인함
