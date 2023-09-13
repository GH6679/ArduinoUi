package com.example.demo.restcontroller;

import com.example.demo.domain.dto.ArduinoUiDto;
import com.fazecast.jSerialComm.SerialPort;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpServletRequest;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

@RestController
@Slf4j
@RequestMapping("/arduino")
public class ArduinoRestUiController {
    private SerialPort serialPort;
    private OutputStream outputStream;
    private InputStream inputStream;

    ArduinoUiDto dto = new ArduinoUiDto();



    @GetMapping("/conn/{COM}")
    public ResponseEntity<String> ArduinoSetConnection(@PathVariable("COM")String COM, HttpServletRequest request){
        log.info("GET /arduino/coon..." + COM);

        if(serialPort != null){ //serialport 에 이전 연결이 있으면 제거
            serialPort.closePort();
            serialPort = null;
        }

        //연결
        serialPort = SerialPort.getCommPort(COM);

        //연결 기본 셋팅
        serialPort.setBaudRate(9600); //통신 속도 설정
        serialPort.setNumDataBits(8); //데이터 비트의 수를 설정 (각 문자를 나타내는 비트의 수)
        serialPort.setNumStopBits(0); //정지 비트 설정  (데이터 전송의 끝을 나타내는 비트)
        serialPort.setParity(0); // 패리티 설정 (데이터 유효성을 확인하는 체크 비트)
        // serialPort의 타임아웃 설정 TIMEOUT_READ_BLOCKING 는 데이터를 읽을때 블로킹 방식으로 대기한다 2000는 타임 아웃 시간
        serialPort.setComPortTimeouts(SerialPort.TIMEOUT_READ_BLOCKING,2000,0);

        //serialPort 열기
        boolean isOpen = serialPort.openPort();
        log.info("serial OPEN : "+isOpen);

        if(isOpen) {
            this.inputStream = serialPort.getInputStream();
            this.outputStream = serialPort.getOutputStream();
            //================================================================
            //수신 스레드
            Worker worker = new Worker();
            Thread th = new Thread(worker);
            th.start();
            //================================================================

            //연결 성공시
            return new ResponseEntity("Connection OK", HttpStatus.OK);
        }else {
            //연결 실패시
            return new ResponseEntity("Connection FAIL", HttpStatus.BAD_GATEWAY);
        }

    }

    //LED 컨트롤러
    @GetMapping("/led/{value}")
    public void led_Control(@PathVariable("value")String value) throws IOException {
        log.info("GET /arduino/led/value..."+value);
        if(serialPort.isOpen()){
            outputStream.write(value.getBytes());
        }

    }


    @GetMapping("/ui/led")
    public String ui_Led(){
        return dto.getUiLed();
    }
    @GetMapping("/ui/tmp")
    public String ui_Tmp(){
        return dto.getUiTmp();
    }
    @GetMapping("/ui/light")
    public String ui_Light(){
        return dto.getUiLight();
    }
    @GetMapping("/ui/dis")
    public String ui_Dis(){
        return dto.getUiDis();
    }


    //수신 스레드 용 클레스
    //================================================================
    class Worker implements Runnable{

        DataInputStream dis;


        @Override
        public void run() {
            dis = new DataInputStream(inputStream);

            try {
                while (!Thread.interrupted()) {
                    if (dis != null) {
                        String data = dis.readLine();
                        System.out.println("[DATA] : " + data);

                        String[] dataIn = data.split("_");

                        try { //Index 1 out of bounds for length 1 예외로 멈추는거 방지
                        if (dataIn.length > 3) {
                            //led 입력이 있을경우
                            dto.setUiLed(dataIn[0]);
                            dto.setUiTmp(dataIn[1]);
                            dto.setUiLight(dataIn[2]);
                            dto.setUiDis(dataIn[3]);
                        } else {
                            //led 입력이 없는 경우
                            dto.setUiTmp(dataIn[0]);
                            dto.setUiLight(dataIn[1]);
                            dto.setUiDis(dataIn[2]);
                        }

                        }catch (ArrayIndexOutOfBoundsException e){
                            e.printStackTrace();
                        }

                    }
                    Thread.sleep(250);
                }
            }catch ( Exception e){
                e.printStackTrace();
            }
        }
    }
    //================================================================

}
