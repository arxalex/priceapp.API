<?php

namespace framework\entities\tokens;

use DateTime;
use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class TokensService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "tokens\\Token";
        $this->tableName = "pa_tokens";
    }
    public function isTokenValid(int $userid, string $token, int $expires): bool
    {
        $tokenInstance = $this->getTokenFromDb($userid, $token, $expires);
        return $tokenInstance != null;
    }
    public function getTokenFromDb(int $userid, string $token, int $expires): Token
    {
        if ($userid >= 0 && $expires >= time() && count_chars($token) == 32) {
            $tokens = $this->getItemsFromDB([
                "userid" => [$userid],
                "token" => [$token],
                "expires" => [$expires]
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
    public function reValidToken(int $userid, string $token, int $expires): Token
    {
        if($this->isTokenValid($userid, $token, $expires)){
            $oldToken = $this->getTokenFromDb($userid, $token, $expires);
            if($this->deleteItem($oldToken)){
                return $this->createToken($userid);
            }
        }
        return null;
    }
    public function createToken(int $userid) : Token{
        $token = StringHelper::generateRsndomString(32);
        $expires = time() + 604800;

        $tokenInstance = new Token(null, $userid, $token, $expires);

        $this->insertItemToDB($tokenInstance);

        return $tokenInstance;
    }
}
