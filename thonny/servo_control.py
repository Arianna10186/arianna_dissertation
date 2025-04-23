import time
import machine
from servo import Servo

uart = machine.UART(1, baudrate=115200, tx=machine.Pin(4), rx=machine.Pin(5))
my_servo = Servo(pin_id=16)

print('Waiting for data...')

def is_number(s):
    try:
        float(s)
        return True
    except:
        return False
# add delay
last_processed_time = time.ticks_ms()
interval_ms = 100 

while True:
    if uart.any():
        current_time = time.ticks_ms()
        if time.ticks_diff(current_time, last_processed_time) >= interval_ms:
            incoming = uart.read()
            if incoming:
                try:
                    data = incoming.decode('utf-8').strip() 
                    if data == '':
                        continue

                    print(f"Received: {data}")

                    if is_number(data):
                        angle = float(data)
                        mapped = (angle + 90) * 1.0
                        mapped = max(0, min(180, mapped))
                        print(f"Move servo to {mapped}")
                        my_servo.write(mapped)
                        last_processed_time = current_time
                    else:
                        print(f"Not a number: {data}")

                except Exception as e:
                    print(f"Error reading data: {e}")

    time.sleep(0.05)
