######### ignore #########
import time
# import network
import socket 
import struct
from servo import Servo

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, 65536)  # Increase buffer size
sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, 65536)  # Increase send buffer size

# assign IP address and port number
server_address = ('10.192.78.205', 10000)
sock.bind(server_address)

# Create Servo object, assigning the
# GPIO pin connected the PWM wire of the servo
my_servo = Servo(pin_id=16)

print('Waiting for UDP data...')

# delay_ms = 25  # Amount of milliseconds to wait between servo movements

while True:
    try:
        # recieve data
        data, address = sock.recvfrom(1024)
        print(f'Received {data}')

        # convert to angle
        '''
        try:
            angle = float(data.decode('utf-8').strip())
            angle = max(0, min(180, angle))
            print(f'Move servo to {angle}')
            my_servo.write(angle)
        except ValueError:
            print(f'Invalid input: {data}')
            '''

    except KeyboardInterrupt:
        print('Exiting...')
        break

    # for position in range(0, 180):  # Step the position forward from 0deg to 180deg
    #     print(position)  # Show the current position in the Shell/Plotter
    #     my_servo.write(position)  # Set the Servo to the current position
    #     time.sleep_ms(delay_ms)  # Wait for the servo to make the movement
    
    # for position in reversed(range(0, 180)):  # Step the position reverse from 180deg to 0deg
    #     print(position)  # Show the current position in the Shell/Plotter
    #     my_servo.write(position)  # Set the Servo to the current position
    #     time.sleep_ms(delay_ms)  # Wait for the servo to make the movement 


sock.close()
