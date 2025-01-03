from flask import Flask
from flask import request
import sys
import os
import time
import google.generativeai as genai


app = Flask(__name__)
print (os.environ.get('GOOGLE_API_KEY'))
genai.configure(api_key=os.environ.get('GOOGLE_API_KEY'))
model = genai.GenerativeModel("gemini-1.5-flash")

@app.route('/')
def hello_world():
    timeStart = time.perf_counter()
    timeEnd = time.perf_counter()
    return str(timeEnd - timeStart)

@app.route('/generate')
def generate_response():
    query = request.args.get('query')
    instrunctions = request.args.get('instructions')

    if not query or not instrunctions:
        return "Invalid request"

    prompt = f"You are a data scientist. You will be given instructions and you need to extract essential information from it. The data will be related to countires. Adhere to the instructions as closely as you can. The instructions will be provided after Instrunctions: and the data will be provided after Data:\nInstructions: {instrunctions}\nData: {query}.Now provide the information that is requested."
    print("Prompt: ", prompt)
    response = model.generate_content(prompt, generation_config=genai.types.GenerationConfig(temperature=0.0))
    print(response.text)
    return response.text

@app.route('/ExtractFromMd')
def ExtractFromMd():
    md = request.args.get('md')
    instructions = request.args.get('query')

    if not md or not instructions:
        return 'Invalid request'

    prompt = f"You are a data scientist. You will be given Markdown file and you need to extract essential information from it. The data will be related to countires. Adhere to the instructions as closely as you can. The instructions will be provided after Instrunctions: and the mark down file will be provided after Data:\nInstructions: {instrunctions}\nData: {query}.Now provide the information that is requested."
    response = model.generate_content(prompt, generation_config=genai.types.GenerationConfig(temperature=0.0))
    return response.text
    