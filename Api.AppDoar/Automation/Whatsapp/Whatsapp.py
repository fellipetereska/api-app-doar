# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.chrome.service import Service  
from webdriver_manager.chrome import ChromeDriverManager  

import csv
import os
import sys
import time
import asyncio

class Whatsapp:
    def __init__(self, executable_path=None, silent=True, headless=True):
        self.options = webdriver.ChromeOptions()
        self.options.binary_location = r"C:\Program Files\Google\Chrome\Application\chrome.exe"

        if silent:
            self.__addOption("--log-level=3")
        if headless:
            self.__addOption("--headless")
            self.__addOption("--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36")
            self.__addOption("--window-size=1920,1080")
            self.__addOption("--no-sandbox")
            self.__addOption("--disable-dev-shm-usage")

        self.__addOption("user-data-dir={}".format(os.path.join(sys.path[0], "UserData")))
        self.__addOption("--disable-gpu")
        self.__addOption("--disable-infobars")
        self.__addOption("--disable-extensions")

        try:
            if executable_path:
                service = Service(executable_path=executable_path)
            else:
                service = Service(ChromeDriverManager().install())
                
            self.browser = webdriver.Chrome(service=service, options=self.options)
            self.browser.maximize_window()
        except Exception as e:
            raise Exception(f"Erro ao iniciar o WebDriver: {str(e)}")

    def __addOption(self, option):
        self.options.add_argument(option)

    def login(self):
        try:
            self.browser.get('https://web.whatsapp.com')
            if not self.__isLogin():
                print("Por favor, escaneie o QR Code")
                time.sleep(3)
                qr_path = os.path.join(sys.path[0], "QRCode.png")
                self.browser.save_screenshot(qr_path)
                print(f"QR Code salvo em: {qr_path}")
                
                while not self.__isLogin():
                    time.sleep(1)
                print("Login realizado com sucesso!")
            else:
                print("Sessao ja  autenticada")
        except Exception as e:
            raise Exception(f"Erro durante o login: {str(e)}")

    def __isLogin(self):
        try:
            self.browser.find_element(By.CLASS_NAME, "landing-wrapper")
            return False
        except:
            try:
                self.browser.find_element(By.CLASS_NAME, "two")
                return True
            except:
                time.sleep(1)
                return False

    def sendMessageToNumber(self, phone_number, message, wait_time=30):
        try:
            self.browser.get(f'https://web.whatsapp.com/send?phone={phone_number}')
            
            self.__waitForChatToLoad(wait_time)
            
            input_box = self.__findMessageInput()
            
            input_box.send_keys(message + Keys.ENTER)
            print(f"Mensagem enviada para {phone_number}")
            
            time.sleep(2)
            return True
        except Exception as e:
            print(f"Erro ao enviar mensagem: {str(e)}")
            self.browser.save_screenshot("error_send_message.png")
            return False

    def __waitForChatToLoad(self, timeout=30):
        try:
            WebDriverWait(self.browser, timeout).until(
                EC.presence_of_element_located((By.XPATH, '//div[@contenteditable="true"][@data-tab="10"]')))
        except:
            raise Exception("Tempo excedido ao carregar o chat")

    def __findMessageInput(self):
        try:
            return self.browser.find_element(By.XPATH, '//div[@contenteditable="true"][@data-tab="10"]')
        except:
            raise Exception("Caixa de mensagem nao encontrada")

    def close(self):
        try:
            self.browser.quit()
        except:
            pass