import socket
import time
import random as r
def tcp_client(host, port, message):
    # Create a TCP/IP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.settimeout(100)  # Set a timeout for operations
    sock.connect((host, port))
    print(f"Connected to {host}:{port}")
    try:
        i = 1
        while True:
            try:
                # Connect the socket to the server if connection is not established
                # if not hasattr(sock, '_connected') or not sock._connected:
                    
                
                # Send data
                i1 = input("Enter the number of data you want to send: ")
                sock.sendall(i1.encode())
                print(f"Sent: {message}")
                
                # Receive data if aviailable
                received = sock.recv(1024)
                print(f"Received: {received.decode()}")
                
                # i += 5
                # print(f"Waiting for {i} seconds")
                # time.sleep(i)

            except socket.timeout:
                print("Operation timed out. Retrying...")
                sock.close()
                sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                sock.settimeout(10)
            
            except ConnectionResetError:
                print("Connection reset by peer. Reconnecting...")
                sock.close()
                sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                sock.settimeout(10)

    except KeyboardInterrupt:
        print("Program interrupted by user.")

    finally:
        # Clean up the connection
        sock.close()
        print("Connection closed")

# Usage
host = "172.36.0.159"  # Replace with the server's hostname or IP address
port = 6988          # Replace with the server's port number
message = "1118310XELT26,1118310XELT26/SFZL/210608A0001"

tcp_client(host, port, message)