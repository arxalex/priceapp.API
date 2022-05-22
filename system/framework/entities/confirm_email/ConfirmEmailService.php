<?php

namespace framework\entities\confirm_email;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class ConfirmEmailService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "confirm_email\\ConfirmEmail";
        $this->tableName = "pa_confirm_email";
    }
    public function isTokenValid(int $userid, string $token): bool
    {
        $tokenInstance = $this->getTokenFromDb($userid, $token);
        return $tokenInstance != null;
    }
    public function getTokenFromDb(int $userid, string $token)
    {
        if ($userid >= 0 && strlen($token) == 32) {
            $tokens = $this->getItemsFromDB([
                "userid" => [$userid],
                "token" => [$token]
            ]);
            if (count($tokens) == 1) {
                return $tokens[0];
            } else {
                return null;
            }
        } else {
            return null;
        }
    }
    public function createToken(int $userid, string $email): ConfirmEmail
    {
        $token = StringHelper::generateRsndomString(32);

        $tokenInstance = new ConfirmEmail(null, $userid, $token);

        $this->insertItemToDB($tokenInstance);

        $link = "https://priceapp.arxalex.co/register/confirm_email?userid=$userid&token=$token";

        $subject = 'Підтвердження реєстрації у PriceApp';
        $message = "
            <h1>Вітаємо!</h1>
            <p>Нещодавно ви зареєструвались на сайті <a href='https://priceapp.arxalex.co'>priceapp.arxalex.co</a></p>
            <p>Для завершення реєстрації необхідно підтвердити адресу електронної пошти. Для цього перейдіть за посиланням: <a href='$link'>$link</a></p>
            <p>Якщо цей лист надійшов до вас помилково, будь-ласка, напишіть на <a href='mailto:info@arxalex.co'>info@arxalex.co</p>
            <p>Дякуємо та бажаємо гарного дня!</p>
            <p>З повагою,<br>
            <a href='https://priceapp.arxalex.co'>PriceApp</a><br>
            <a href='https://priceapp.arxalex.co'><img src='https://priceapp.arxalex.co/public_resources/icons/priceapp_icon.svg' width='100' height='100'/></a><br>
            <a href='mailto:info@arxalex.co'>info@arxalex.co</p>
        ";
        $headers =  'From: PriceApp <info@arxalex.co>' . "\r\n" .
            'Reply-To: PriceApp <info@arxalex.co>' . "\r\n" .
            'X-Mailer: PHP/' . phpversion() . "\r\n" .
            'MIME-Version: 1.0' . "\r\n" .
            'Content-type: text/html; charset=utf-8' . "\r\n";
        mb_send_mail($email, $subject, $message, $headers);

        return $tokenInstance;
    }
    public function deleteToken(int $userid, string $token): bool
    {
        if ($this->isTokenValid($userid, $token)) {
            $oldToken = $this->getTokenFromDb($userid, $token);
            return $this->deleteItem($oldToken);
        }
        return false;
    }
}
