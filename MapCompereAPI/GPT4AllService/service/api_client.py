from flask import Flask
from flask import request
import sys
import os
import time

llm_client_path = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
sys.path.append(llm_client_path)

from llm_client import LLMClient

app = Flask(__name__)
llm_client = LLMClient()

@app.route('/')
def hello_world():
    timeStart = time.perf_counter()
    response = llm_client.generate("Hi there!")
    timeEnd = time.perf_counter()
    return response + str(timeEnd - timeStart)

@app.route('/generate')
def generate_response():
    query = request.args.get('query')
    instrunctions = request.args.get('instructions')
    print("Querry: ", query)
    print("Instructions: ", instrunctions)
    if not query or not instrunctions:
        return "Invalid request"
    prompt = f"Instructions: {instrunctions}\nData: {query}"
    response = llm_client.generate(prompt)
    print(response)
    return response