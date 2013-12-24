Examonitor
=======================
<h2>Concept</h2>
<h3>Algemeen</h3>
<p>De applicatie maakt het mogelijk om toezichtsbeurten te registreren voor de docenten. Docenten dienen een account te 

registreren, daarna kunnen ze inloggen. Enkel email adressen de eindigen in <i>@ap.be</i> worden toegelaten. Eenmaal ingelogd 

krijgen ze een lijst van examens met het benodigde aantal toezichters, ze kunnen door middel van een knop zich al toezichter 

registreren aan dat bepaalde examen.</p>
<p>De administrators kunnen monitorbeurten (examens) aanmaaken alsook campussen.</p>

<h3>Roles</h3>
<p>In deze applicatie wordt gebruik gemaakt van roles. Er is een docent role en een admin role. De docent kan enkel zichzelf als 

toezichter registreren. Elke administrator kan dit ook, maar kan hiernaast ook alle registraties uitlezen, monitorbeurten en 

campussen aanmaken, en zelf de roles van andere gebruikers aanpassen.</p>

<h3>Berichten<h3>
<p>De administrators kunnen een bericht ingeven dat aan alle gebruikers wordt getoond. Deze berichten worden beheerd via een 

aparte pagina in het adminpaneel, zo kunnen berichten worden aangemaakt en verwijderd en worden gemaakt.</p>

<h3>Import</h3>
<p>De administrators kunnen een JSON bestand importeren om de verschillende examens en campussen sneller toe te voegen.</p>
<p>Voorbeeld JSON file:</p>
<p><i><pre>{
	&quot;monitorbeurten&quot;: [
		{
			&quot;monitorbeurt&quot;: 
			{
			  &quot;ExamenNaam&quot;: &quot;Monitorbeurt 1&quot;,
			  &quot;BeginDatum&quot;: &quot;2013/10/18 08:00 AM&quot;,
			  &quot;EindDatum&quot;: &quot;2013/10/18 08:00 PM&quot;,
			  &quot;Capaciteit&quot;: 10,
			  &quot;Digitaal&quot;: false,
			  &quot;Campus&quot;: &quot;Campus 1&quot;
			}
		},
		{
			&quot;monitorbeurt&quot;: 
			{
			  &quot;ExamenNaam&quot;: &quot;Monitorbeurt 2&quot;,
			  &quot;BeginDatum&quot;: &quot;2013/10/18 08:00 AM&quot;,
			  &quot;EindDatum&quot;: &quot;2013/10/18 08:00 PM&quot;,
			  &quot;Capaciteit&quot;: 20,
			  &quot;Digitaal&quot;: true,
			  &quot;Campus&quot;: &quot;Campus 2&quot;
			}
		}
	]
}</pre></i></p>

<h2>Installatie</h2>
<h3>Web deploy</h3>
<p>Om deze applicatie te isntalleren is er een IIS server nodig (versie 7.5 of hoger) en een SQL database.</p>
<p>De gemakkelijkste manier om de installatie uit te voeren is door het project te openen in Visual Studio 2013. Hierna kan met 

een web deploy de applicatie worden geïnstalleerd. Er moet enkel een lege database worden aangemaakt omdat de database wordt 

aangemaakt door de code first migrations. Dit moet dus zeker worden aangevinkt bij de settings.</p>
<p>Verder is het balngrijk om te zorgen dat de applicatie draait op .NET 4.5, en dat de gebruikersaccount dat hiervoor wordt 

gebruikt ook rechten heeft op de SQL database.</p>
<p>Door de code first migrations wordt de <i>Admin</i> account aangemaakt met als wachtwoord <i>123456</i>, aangeraden wordt dit 

zo snel mogelijk aan te passen. Hiernaast worden ook enkele test accounts aangemaakt (user1@ap.be, user2@ap.be, user3@ap.be, 

wachtwoord <i>password</i>, alsook enkele monitorbeurten en campussen. Deze kunnen gemakkelijk worden verwijderd via de Admin 

account nadat is geverifïeerd dat alles werkt.</p>

<h3>web.config instellingen</h3>
<h4>Admin instellingen</h4>
<p>Dit email adres wordt gebruikt voor het versturen van bevestingingsmails. Als je het wachtwoord van de Admin account bent 

vergeten wordt het ook naar dit email adres verstuurd.</p>
<p>Om het adres te wijzigen dient het volgende aangepast te worden:</p>

<p><i>
&lt;appSettings&gt;<br />
    &lt;add key=&quot;AdminEmail&quot; value=&quot;examonitor@ap.be&quot; /&gt;<br />
&lt;/appSettings&gt;<br />
</i></p>

<p>Indien je het wachtwoord van de Admin account bent vergeten en een nieuw wil aanvragen moet je gebruik maken van deze 

controller: <i>../Account/ResetAdminPassword</i>. Dit omdat de normale wachtwoord reset controller niet zal werken voor de Admin 

account.</p>


<h4>Mail server instellen</h4>
<p>Om het versturen van mails door de applicatie mogelijk te maken dienen de volgende instellingen te worden aangepast aan de 

configuratie van de eigen mailserver</p>
<p><i>
&lt;system.net&gt;<br />
    &lt;mailSettings&gt;<br />
      &lt;smtp deliveryMethod=&quot;SpecifiedPickupDirectory&quot;&gt; <br />
        &lt;specifiedPickupDirectory pickupDirectoryLocation=&quot;c:\email&quot; /&gt; <br />
        &lt;network host=&quot;localhost&quot; /&gt;<br />
      &lt;/smtp&gt;<br />
    &lt;/mailSettings&gt;<br />
  &lt;/system.net&gt;<br />
</i></p>

