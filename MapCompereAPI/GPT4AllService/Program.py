from gpt4all import GPT4All

modelName = "mistral-7b-instruct-v0.1.Q4_0.gguf"
modelPath = f"models/{modelName}"

model = GPT4All(model_name=modelName, model_path=modelPath, device="gpu")

with model.chat_session():
    output = model.generate("Hi, how are you?")
    print(output)