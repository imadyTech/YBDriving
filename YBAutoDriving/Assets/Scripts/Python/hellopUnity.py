import socket, threading, sys

UnityIP = '127.0.0.1'   
UnityPort = 31415       
PYTHON_LISTEN_IP = '127.0.0.1'
PYTHON_LISTEN_PORT = 31416
BufferSize = 24_576   # the resolution of input image 128x64x3

def send_To_Unity(command, message):
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as sock:
        #sock.bind((UnityIP, UnityPort))
        # Encode the message to bytes and send it to Unity
        bytes_message = message.encode('utf-8')
        sock.sendto(bytes_message, (UnityIP, UnityPort))
        #print(f"Message sent to Unity: {len(bytes_message)}")


def listen_from_unity(on_message_received_handler):
    """
    Listen for a response from Unity using UDP and use a callback function when a message is received.

    Parameters:
    - on_message_received: A callback function that takes the received message as its parameter.
    """
    print(f"Listening for responses on {PYTHON_LISTEN_IP}:{PYTHON_LISTEN_PORT}")
    sys.stdout.flush()

    sock1= socket.socket(socket.AF_INET, socket.SOCK_DGRAM) 
    sock1.bind((PYTHON_LISTEN_IP, PYTHON_LISTEN_PORT))
    while True:
        data, addr = sock1.recvfrom(BufferSize)
        on_message_received_handler(data)
        #print(f"Received response from Unity: {len(data)} bytes")
        #sys.stdout.flush()

    # Start listening in a separate thread
    #thread = threading.Thread(target=listen, daemon=True)
    #thread.start()
    #return thread  # Optional: return the thread if you need to manage it

def handle_received_binary_data(data):
    # Here you can process the received binary data
    #print(f"Received data length: {len(data.decode())} bytes")
    #sys.stdout.flush()
    send_To_Unity(f"Received data length: {len(data.decode())} bytes")

if __name__ == "__main__":
    #sendToUnity("Python is standing by...")
    listen_from_unity(handle_received_binary_data)
