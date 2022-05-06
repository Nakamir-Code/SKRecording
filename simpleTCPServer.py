import socket


class SimpleTCPServer():
    def __init__(self, IP, PORT) -> None:
        self.incomplete_JSON = None
        self.recording = []
        # Create a TCP/IP socket
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.serverInfo = (IP, PORT)

    def parseJSON(self, packet):
        # print("....................................................................................................")
        # print("Packet: ")
        # print(packet)

        frames = packet.split(b';')
        # print("Frame: ")
        # print(frames)

        start = 0
        end = len(frames)

        # Frame 0's, rare situation handling.
        if len(frames[0]) == 0:  # we got a packet starting with ;
            start = 1
        elif frames[0][0] != ord('{'):  # the first frame is the missing part of last packet's last frame.
            frames[0] = self.incomplete_JSON + frames[0]
            self.incomplete_JSON = None
        
        last_frame = frames[len(frames)-1]

        # Last frame's rare situation handling.
        if len(last_frame) == 0:  # The packet ended with ;
            end = len(frames) - 1
        elif last_frame[len(last_frame)-1] != ord('}'):  # the last frame contained first half of a frame (json string)
            self.incomplete_JSON = last_frame
            end = len(frames) - 1

        return frames[start:end]  # return list of full json strings

    def start(self):
        print("Starting to listen")
        self.sock.bind(self.serverInfo)
        self.sock.listen(1)

        while True:
            connection, client_address = self.sock.accept()
            try:
                print('client connected:', client_address)
                while True:
                    data = connection.recv(10000)

                    # P for playback
                    if data[0] == ord('P'):
                        # Send the total length first
                        LByte = bytes("L", 'utf-8')
                        frameCount = len(self.recording)
                        fcbytes = frameCount.to_bytes(4, 'little')
                        connection.sendall(LByte + fcbytes)            

                        for frame in self.recording:
                            semicolonByte = bytes(";", 'utf-8')
                            connection.sendall(frame + semicolonByte)
                    else:
                        frames = self.parseJSON(data)
                        for frame in frames:
                            self.recording.append(frame)

            except:
                print("Client disconnected")
                connection.close()

def main():
    server = SimpleTCPServer("10.0.0.4", 12345)
    server.start()

if __name__ == "__main__":
    main()
