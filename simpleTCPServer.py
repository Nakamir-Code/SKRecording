import socket

IP = "10.0.0.5"
PORT = 12345

recording = []
# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


server_address = (IP, PORT)
print("Starting to listen")
sock.bind(server_address)
sock.listen(1)

while True:
    connection, client_address = sock.accept()
    try:
        print('client connected:', client_address)
        while True:
            data = connection.recv(10000)
            # P for playback
            if data[0] == ord('P'):
                # Send the total length first
                LByte = bytes("L", 'utf-8')
                frameCount = len(recording)
                print(frameCount, flush=True)
                fcbytes = frameCount.to_bytes(4, 'little')
                connection.sendall(LByte + fcbytes)            

                for frame in recording:
                    semicolonByte = bytes(";", 'utf-8')
                    connection.sendall(frame + semicolonByte)
            else:
                recording.append(data)

    finally:
        connection.close()
