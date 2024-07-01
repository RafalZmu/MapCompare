from gpt4all import GPT4All
import pandas as pd

class LLMClient:
    def __init__(self, device="gpu"):
        self.system_prompt =  '### System:\nYou are a data scientist. You will be given csv file and you need to extract essential information from it get the example column names. The data will be related to countires. Extract what the data is about'
        self.model_name = "mistral-7b-instruct-v0.1.Q4_0.gguf"
        self.model_path =r"D:\Projekty\Praca_inz\MapCompare\MapCompereAPI\GPT4AllService\Models"
        print('Initializing model')
        self.model = GPT4All(model_name=self.model_name, model_path=self.model_path, device=device)
        print('Model initialized')

    def generate(self, prompt):
        with self.model.chat_session(system_prompt=self.system_prompt):
            output = self.model.generate(prompt, max_tokens=5000, temp=0, )
            return output