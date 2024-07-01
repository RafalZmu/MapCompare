from flask import Flask
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