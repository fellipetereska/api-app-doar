# -*- coding: utf-8 -*-
import sys
import time
from Whatsapp import Whatsapp
from selenium.common.exceptions import WebDriverException

def enviar_mensagem(numero, mensagem, tentativas=3):
    bot = None
    for tentativa in range(tentativas):
        try:
            print(f"Tentativa {tentativa + 1} de {tentativas}")
            
            bot = Whatsapp(silent=True, headless=False)
            
            bot.login()
            
            success = bot.sendMessageToNumber(numero, mensagem)
            
            if success:
                time.sleep(5)
                return True
            
        except WebDriverException as e:
            print(f"Erro no WebDriver: {str(e)}")
            if 'chrome not reachable' in str(e).lower():
                print("Reiniciando o navegador...")
        except Exception as e:
            print(f"Erro inesperado: {str(e)}")
        finally:
            if bot:
                bot.close()
            time.sleep(10)
    
    return False

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Uso: python bot.py <numero> <mensagem>")
        print("Exemplo: python bot.py 554399999999 \"Ola, sua doacao foi aceita!\"")
        sys.exit(1)
    
    numero = sys.argv[1].strip()
    mensagem = ' '.join(sys.argv[2:]).strip()
    
    if not numero.startswith('55'):
        numero = '55' + numero.lstrip('0')
    
    print(f"Enviando mensagem para {numero}...")
    
    if enviar_mensagem(numero, mensagem):
        print("Mensagem enviada com sucesso!")
        sys.exit(0)
    else:
        print("Falha ao enviar mensagem apos varias tentativas")
        sys.exit(1)