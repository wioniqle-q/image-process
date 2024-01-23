import time
import cv2
import requests
import threading
import io

class BaseClass:
    def __init__(self, _api_url, camera_index=0, wait_interval=0.1):
        self.thread = None
        self.api_url = _api_url
        self.cap = cv2.VideoCapture(camera_index)
        self.is_running = False
        self.lock = threading.Lock()
        self.wait_interval = wait_interval

        if not self.cap.isOpened():
            raise Exception("Unable to open the camera")

    def start(self):
        self.is_running = True
        self.thread = threading.Thread(target=self._stream)
        self.thread.start()

    def stop(self):
        self.is_running = False
        self.thread.join()

    def _stream(self):
        try:
            while self.is_running:
                ret, frame = self.cap.read()

                if not ret:
                    print("Unable to read frame from the camera")
                    break

                _, img_encoded = cv2.imencode('.jpg', frame)
                image_bytes = img_encoded.tobytes()
                image_file = io.BytesIO(image_bytes)
                files = {"imageFile": image_file}

                start_time = time.time()

                try:
                    with self.lock:
                        response = requests.post(self.api_url, files=files)
                        print(response.text)
                        elapsed_time = (time.time() - start_time) * 1000
                        print(f"Request completed in {elapsed_time:.2f} ms")

                except requests.RequestException as e:
                    print(f"Error making the request {e}")

                time.sleep(self.wait_interval)

        except Exception as e:
            print(f"An error occurred: {e}")

        finally:
            self.cap.release()

if __name__ == "__main__":
    api_url = "http://localhost:5157/processimage/ProcessImage"

    stream = None

    try:
        stream = BaseClass(api_url)
        stream.start()

        threading.Event().wait(30 * 1000)  # 30 * 1000 = 30 second or 60 * 60 * 1000 = 1 hour or 2 * 60 * 60 * 1000 = 2 hours

    finally:
        stream.stop()
