Coinone Alarm v1.0.0.0

[목적]
- Coinone Api를 사용해 제공되는 코인들의 가격을 알림으로 받기 위함
- Coinone 거래소를 통해 제공되는 코인들의 24시간 상/하한가 제공
- 알림을 설정하는 경우, 설정 가격 이하로 떨어지면 Windows 알림 제공
- 사용자의 Access Token 인증시 각 코인의 보유량 제공

[설명]
- .NET Framework 4.5 필요
- 소스는 Visual Studio 2012 이상으로 빌드
- 프로그램 실행시마다 서로 다른 랜덤한 키를 생성하여 사용자 Access Token과 ApiKey 암호화
