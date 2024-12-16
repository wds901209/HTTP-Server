[tester::#QV8] Running tests for Stage #QV8 (Read request body)
[tester::#QV8] $ ./your_program.sh --directory /tmp/data/codecrafters.io/http-server-tester/
[your_program] Logs from your program will appear here!
[tester::#QV8] $ curl -v -X POST http://localhost:4221/files/raspberry_banana_pear_apple -H "Content-Length: 59" -H "Content-Type: application/octet-stream" -d 'pineapple banana mango apple orange mango orange strawberry'
[your_program] Time of accepting the socket: 12/16/2024 07:27:03
[your_program] Received data: POST /files/raspberry_banana_pear_apple HTTP/1.1
[your_program] Host: localhost:4221
[your_program] Content-Length: 59
[your_program] Content-Type: application/octet-stream
[your_program] 
[your_program] pineapple banana mango apple orange mango orange strawberry
[your_program] Request Line: POST /files/raspberry_banana_pear_apple HTTP/1.1
[your_program] 
[your_program] Http Method: POST
[your_program] Http Target: /files/raspberry_banana_pear_apple
[your_program] Http Version: HTTP/1.1
[your_program] 
[your_program] Response: HTTP/1.1 201 Created
[your_program] 
[your_program] 
[tester::#QV8] Received response with 201 status code
[tester::#QV8] Test passed.