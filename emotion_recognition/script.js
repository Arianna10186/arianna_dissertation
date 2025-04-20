const video = document.getElementById('video')

Promise.all([
  faceapi.nets.tinyFaceDetector.loadFromUri('/models'),
  faceapi.nets.faceLandmark68Net.loadFromUri('/models'),
  faceapi.nets.faceRecognitionNet.loadFromUri('/models'),
  faceapi.nets.faceExpressionNet.loadFromUri('/models')
]).then(startVideo)

function startVideo() {
    navigator.mediaDevices.getUserMedia({ video: true })
      .then(stream => {
        video.srcObject = stream;
      })
      .catch(err => {
        console.error("Error accessing the camera: ", err);
      });
  }

video.addEventListener('play', () => {
  const canvas = faceapi.createCanvasFromMedia(video)
  document.body.append(canvas)
  const displaySize = { width: video.width, height: video.height }
  faceapi.matchDimensions(canvas, displaySize)
  
  setInterval(async () => {
    const detections = await faceapi.detectAllFaces(video, new faceapi.TinyFaceDetectorOptions()).withFaceLandmarks().withFaceExpressions()
    
    const resizedDetections = faceapi.resizeResults(detections, displaySize)
    
    // get face expression classes
    const expressions = resizedDetections.map(d => d.expressions)
    // find class closest to 1
    const closestExpressions = expressions.map(exp => {
      return Object.keys(exp).reduce((closestExpression, currentExpression) => {
        return Math.abs(exp[closestExpression] - 1) < Math.abs(exp[currentExpression] - 1) ? closestExpression : currentExpression;
      });
    });
    console.log(closestExpressions[0])
    // send classification results to server
    fetch('http://localhost:5000/detect', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(closestExpressions)
      }).catch(err => {
        console.error("Error sending data", err)
      })
      
    canvas.getContext('2d').clearRect(0, 0, canvas.width, canvas.height)
    faceapi.draw.drawDetections(canvas, resizedDetections)
    faceapi.draw.drawFaceLandmarks(canvas, resizedDetections)
    faceapi.draw.drawFaceExpressions(canvas, resizedDetections)
  }, 100)
})