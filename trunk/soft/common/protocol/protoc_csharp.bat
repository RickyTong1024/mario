del ..\..\client\Assets\script\protobuf\*.cs

..\etools\protogen.exe -i:map.proto -o:..\..\client\Assets\script\protobuf\map_t.cs
..\etools\protogen.exe -i:msg.proto -o:..\..\client\Assets\script\protobuf\msg_t.cs

pause
