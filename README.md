# BIL_424_Odev1

<h3>Oyunudaki amaç, mavi varillerin hepsini bulup Yeşil bölgeye getirmek</h3></br>
<ul>
 <li>WASD ile karaketri hareket etme</li>
 <li>Shift + W ile koşma</li>
 <li>Space ile zıplama</li></br>
	
 <li>E ile yerdeki elmasları toplayıp güç kazanmak</li></br>

 <li>Mouse ile etrafa bakma</li>
 <li>Mouse Sağ tık ile MAVİ VARİLLERİ tutup taşıma</li>
 <li>Mouse Sol tık ile Taşınan varilleri fırlatma</li>
</ul>

<h3>Oyunu yaparken zorlandığım noktalar</h3></br>
<ul>
 <p>Karakter,ileri geri harektelerini yaparken bazen yerden 0.2 yukarı kalkıyordu. Kodumu defalarca analiz ettikten sonra yerdeki colliderin Convex olduğunu farkettim. Colliderın convex özelliğini kaldırınca sorun çözüldü.</p>
<p>Diğer zorluk ise çift zıplama bazen çalışmıyordu onu halletmek için biraz zaman harcadım.(IsGrounded kontrolü yapan raycast fonksiyonu ekledim)</p>
<p> Ayrıca harektlere animasyon eklediğim biraz zamanımı aldı.</p>
</ul>
