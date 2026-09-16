import socket
import json
import os
import sys

HOST = '127.0.0.1'
PORT = 9876

def send_command(cmd_type, params=None, timeout=60.0):
    if params is None:
        params = {}
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.settimeout(timeout)
    sock.connect((HOST, PORT))
    
    msg = json.dumps({'type': cmd_type, 'params': params})
    sock.sendall(msg.encode('utf-8'))
    
    chunks = []
    try:
        while True:
            chunk = sock.recv(65536)
            if not chunk:
                break
            chunks.append(chunk)
            buf = b''.join(chunks)
            try:
                res = json.loads(buf.decode('utf-8'))
                sock.close()
                return res
            except json.JSONDecodeError:
                continue
    except socket.timeout:
        print("SOCKET TIMEOUT EXCEEDED")
        pass
    
    sock.close()
    buf = b''.join(chunks)
    if buf:
        try:
            return json.loads(buf.decode('utf-8'))
        except Exception:
            pass
    return {"status": "error", "message": "No response"}

def execute_code(code_str):
    res = send_command('execute_code', {'code': code_str}, timeout=90.0)
    return res

def save_screenshot(filepath, max_size=1000):
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    res = send_command('get_viewport_screenshot', {'filepath': filepath, 'max_size': max_size}, timeout=30.0)
    if res.get("status") == "success" and os.path.exists(filepath):
        print(f"SAVED SCREENSHOT: {filepath}")
        return True
    print("SCREENSHOT FAILED:", res)
    return False

if __name__ == "__main__":
    if len(sys.argv) > 1:
        cmd = sys.argv[1]
        if cmd == "code" and len(sys.argv) > 2:
            print(execute_code(sys.argv[2]))
        elif cmd == "shot" and len(sys.argv) > 2:
            save_screenshot(sys.argv[2])
