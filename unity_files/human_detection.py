import cv2
import pyrealsense2 as rs
import numpy as np

import base64

import socket 
import struct

from ultralytics import YOLO
import time

from datetime import datetime
now = datetime.now()

########## Connect to realsense camera ##########
pipe = rs.pipeline()
config = rs.config() # initiation call to set up settings

config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)
config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30) # type, width, height, format, fps

pipe.start(config) # start streaming
####################

########## Initialise Model ##########
model = YOLO(r'Assets\Scripts\xml_files\person_detect_5_epochs.pt')

#frames = cv2.VideoCapture(0) # webcam

####################

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, 65536)  # Increase buffer size
sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, 65536)  # Increase send buffer size

# assign IP address and port number
server_address = ('10.192.78.205', 10000)
# sock.bind(server_address)

##############
def compute_iou(box1, box2):
    x1, y1, x2, y2 = box1
    x1_, y1_, x2_, y2_ = box2

    # calculate intersection
    xi1, yi1 = max(x1, x1_), max(y1, y1_)
    xi2, yi2 = min(x2, x2_), min(y2, y2_)
    intersection = max(0, xi2 - xi1) * max(0, yi2 - yi1)

    # calculate union
    area1 = (x2 - x1) * (y2 - y1)
    area2 = (x2_ - x1_) * (y2_ - y1_)
    union = area1 + area2 - intersection

    return intersection / union if union != 0 else 0

########## Open Camera and Make Predictions ##########
while True:
    #############
    frames = pipe.wait_for_frames(50000)
    colour_frame = frames.get_color_frame()
    depth_frame = frames.get_depth_frame()

    # convert to numpy array
    colour_image = np.asanyarray(colour_frame.get_data())
    depth_image = np.asanyarray(depth_frame.get_data())
    depth_cm = cv2.applyColorMap(cv2.convertScaleAbs(depth_image, alpha=0.5), cv2.COLORMAP_JET)

    # convert to grayscale
    gray_img = cv2.cvtColor(colour_image, cv2.COLOR_BGR2GRAY)
    ##################

    # for webcam
    #_, frame = frames.read() # uncomment if using webcam
    #results = model(frame, verbose=False)
    results = model(colour_image, verbose=False)

    classes = []
    class_names = ['Person']
    #################

    # draw bounding boxes #################
    # for (x, y, w, h) in bodies:
    #     centre_x = x + w // 2
    #     centre_y = y + h // 2

    #     depth = depth_frame.get_distance(centre_x, centre_y)

    #     # 3d position in pixels
    #     coord = np.array([centre_x, centre_y, depth])
    #     # print(coord)

    #     cv2.rectangle(colour_image, (x, y), (x + w, y + h), (0, 255, 0), 2)
    #     cv2.putText(colour_image, "Person", (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        ##########################

    threshold = 0.2
    boxes = []

    for b in results:
        for c in b.boxes:
            if model.names[int(c.cls)] in class_names:
                # class_name = model.names[int(c.cls)]
                # classes.append(model.names[int(c.cls)])
                x1, y1, x2, y2 = map(int, c.xyxy[0])
                boxes.append((x1, y1, x2, y2))

    filtered_boxes = []
    while boxes:
        current_box = boxes.pop(0)
        keep = True
        for other_box in boxes:
            if compute_iou(current_box, other_box) > threshold:
                keep = False
                break
        if keep:
            filtered_boxes.append(current_box)

    for (x1, y1, x2, y2) in filtered_boxes:
        centre_x = (x1 + x2) // 2
        centre_y = (y1 + y2) // 2

        depth = depth_frame.get_distance(centre_x, centre_y)
        # depth = 0.7

        # 3d position in pixels
        coord = np.array([centre_x, centre_y, depth])
        # print(coord)

        cv2.rectangle(colour_image, (x1, y1), (x2, y2), (0, 255, 0), 2) # green
        cv2.putText(colour_image, "Person", (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        # cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2) # green
        # cv2.putText(frame, "Person", (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

        ########## Send Data through UDP ##########

        message = struct.pack('fff', centre_x, centre_y, depth)
        sock.sendto(message, server_address)
    #     time.sleep(0.01)
    # else:
    #     continue # maybe add smth for when info isn't being sent (or can do that in C#)

    cv2.imshow('Depth', depth_cm)
    cv2.imshow('RGB', colour_image)
    #cv2.imshow('Webcam', frame) 

    if cv2.waitKey(1) == ord('q'):
        break

sock.close()
# pipe.stop()

####################