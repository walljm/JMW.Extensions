namespace JMW.Parsing.Tests
{
    public class NetstatTest
    {
        [Fact]
        public void ParseJsonTest()
        {
            var output = """
netstat -rn
Routing tables
Internet:
Destination        Gateway            Flags               Netif Expire
default            10.181.10.1        UGScg                 en0
default            link#22            UCSIg               utun4
10.181.10/23       link#15            UCS                   en0      !
10.181.10.1/32     link#15            UCS                   en0      !
10.181.10.1        0:0:c:7:ac:b5      UHLWIir               en0   1179
10.181.10.18/32    link#15            UCS                   en0      !
10.181.11.255      ff:ff:ff:ff:ff:ff  UHLWbI                en0      !
100.64/10          link#22            UCS                 utun4
100.100.100.100/32 link#22            UCS                 utun4
100.100.100.100    link#22            UHWIi               utun4
100.107.9.25       100.107.9.25       UH                  utun4
127                127.0.0.1          UCS                   lo0
127.0.0.1          127.0.0.1          UH                    lo0
169.254            link#15            UCS                   en0      !
192.168.1          link#22            UCS                 utun4
192.168.1.70       link#22            UHWIi               utun4
192.168.2          link#22            UCS                 utun4
192.168.10         link#22            UCS                 utun4
224.0.0/4          link#15            UmCS                  en0      !
224.0.0/4          link#22            UmCSI               utun4
239.255.255.250    1:0:5e:7f:ff:fa    UHmLWI                en0
255.255.255.255/32 link#15            UCS                   en0      !
255.255.255.255    ff:ff:ff:ff:ff:ff  UHLWbI                en0      !
255.255.255.255/32 link#22            UCSI                utun4
Internet6:
Destination                             Gateway                                 Flags               Netif Expire
default                                 fe80::%utun0                            UGcIg               utun0
default                                 fe80::%utun1                            UGcIg               utun1
default                                 fe80::%utun2                            UGcIg               utun2
default                                 fe80::%utun3                            UGcIg               utun3
default                                 fd7a:115c:a1e0::                        UGcIg               utun4
::1                                     ::1                                     UHL                   lo0
fd7a:115c:a1e0::/48                     fe80::623e:5fff:fe88:c1a%utun4          Uc                  utun4
fd7a:115c:a1e0::6aeb:919                link#22                                 UHL                   lo0
fe80::%lo0/64                           fe80::1%lo0                             UcI                   lo0
fe80::1%lo0                             link#1                                  UHLI                  lo0
fe80::%ap1/64                           link#14                                 UCI                   ap1
fe80::603e:5fff:fe88:c1a%ap1            62:3e:5f:88:c:1a                        UHLI                  lo0
fe80::%en0/64                           link#15                                 UCI                   en0
fe80::468:ae23:2f6f:2856%en0            60:3e:5f:88:c:1a                        UHLI                  lo0
fe80::c824:c3ff:fea6:76cc%awdl0         ca:24:c3:a6:76:cc                       UHLI                  lo0
fe80::c824:c3ff:fea6:76cc%llw0          ca:24:c3:a6:76:cc                       UHLI                  lo0
fe80::%utun0/64                         fe80::28e2:354b:d10:a172%utun0          UcI                 utun0
fe80::28e2:354b:d10:a172%utun0          link#18                                 UHLI                  lo0
fe80::%utun1/64                         fe80::c60:e4f8:91ee:99e6%utun1          UcI                 utun1
fe80::c60:e4f8:91ee:99e6%utun1          link#19                                 UHLI                  lo0
fe80::%utun2/64                         fe80::dad:d2df:fde5:2dcb%utun2          UcI                 utun2
fe80::dad:d2df:fde5:2dcb%utun2          link#20                                 UHLI                  lo0
fe80::%utun3/64                         fe80::ce81:b1c:bd2c:69e%utun3           UcI                 utun3
fe80::ce81:b1c:bd2c:69e%utun3           link#21                                 UHLI                  lo0
fe80::%utun4/64                         fe80::623e:5fff:fe88:c1a%utun4          UcI                 utun4
fe80::623e:5fff:fe88:c1a%utun4          link#22                                 UHLI                  lo0
ff00::/8                                ::1                                     UmCI                  lo0
ff00::/8                                link#14                                 UmCI                  ap1
ff00::/8                                link#15                                 UmCI                  en0
ff00::/8                                link#16                                 UmCI                awdl0
ff00::/8                                link#17                                 UmCI                 llw0
ff00::/8                                fe80::28e2:354b:d10:a172%utun0          UmCI                utun0
ff00::/8                                fe80::c60:e4f8:91ee:99e6%utun1          UmCI                utun1
ff00::/8                                fe80::dad:d2df:fde5:2dcb%utun2          UmCI                utun2
ff00::/8                                fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff00::/8                                fe80::623e:5fff:fe88:c1a%utun4          UmCI                utun4
ff01::%lo0/32                           ::1                                     UmCI                  lo0
ff01::%ap1/32                           link#14                                 UmCI                  ap1
ff01::%en0/32                           link#15                                 UmCI                  en0
ff01::%utun0/32                         fe80::28e2:354b:d10:a172%utun0          UmCI                utun0
ff01::%utun1/32                         fe80::c60:e4f8:91ee:99e6%utun1          UmCI                utun1
ff01::%utun2/32                         fe80::dad:d2df:fde5:2dcb%utun2          UmCI                utun2
ff01::%utun3/32                         fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff01::%utun4/32                         fe80::623e:5fff:fe88:c1a%utun4          UmCI                utun4
ff02::%lo0/32                           ::1                                     UmCI                  lo0
ff02::%ap1/32                           link#14                                 UmCI                  ap1
ff02::%en0/32                           link#15                                 UmCI                  en0
ff02::%utun0/32                         fe80::28e2:354b:d10:a172%utun0          UmCI                utun0
ff02::%utun1/32                         fe80::c60:e4f8:91ee:99e6%utun1          UmCI                utun1
ff02::%utun2/32                         fe80::dad:d2df:fde5:2dcb%utun2          UmCI                utun2
ff02::%utun3/32                         fe80::ce81:b1c:bd2c:69e%utun3           UmCI                utun3
ff02::%utun4/32                         fe80::623e:5fff:fe88:c1a%utun4          UmCI                utun4
""";

        }
    }
}